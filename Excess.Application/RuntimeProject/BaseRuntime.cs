using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Excess.Entensions.XS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Compilation = Excess.Compiler.Roslyn.Compilation;

namespace Excess.RuntimeProject
{
    internal abstract class BaseRuntime : IRuntimeProject
    {
        protected static Dictionary<string, ICompilationTool> _tools = new Dictionary<string, ICompilationTool>();

        //Console
        protected static SyntaxTree consoleTree = SyntaxFactory.ParseSyntaxTree(
            @"using System;
            public class console
            {
                static private Action<string> _notify;
                static public void setup(Action<string> notify)
                {
                    _notify = notify;
                }

                static public void write(string message)
                {
                    _notify(message);
                }
            }");

        protected static SyntaxTree randomTree = SyntaxFactory.ParseSyntaxTree(
            @"using System;
            public class random
            {
                static private Random _random = new Random();

                static public int Int()
                {
                    return _random.Next();
                }

                static public int Int(int range)
                {
                    return _random.Next(range);
                }

                static public double Double()
                {
                    return _random.NextDouble();
                }
            }");

        protected bool _busy;
        protected Compilation _compilation;
        protected bool _dirty;

        protected Dictionary<string, int> _files = new Dictionary<string, int>();
        private INotifier _notifier;


        public BaseRuntime(IPersistentStorage storage)
        {
            _compilation = new Compilation(storage);
            _compilation.ToolStarted += (sender, args) => Notify(NotificationKind.System, "Starting tool: " + args.Tool.DisplayName + " for " + args.Document);
            _compilation.ToolFinished += (sender, args) =>
            {
                if (args.Result != null)
                {
                    var hasError = args.Result.ContainsKey("error");
                    if (hasError)
                    {
                        Notify(NotificationKind.Error, "Tool failed: " + args.Result["error"]);
                    }
                    else
                    {
                        foreach (var msg in args.Result)
                        {
                            Notify(NotificationKind.System, msg.Key);
                        }
                    }
                }

                Notify(NotificationKind.System, "Finished: " + args.Tool.DisplayName + " for " + args.Document);
            };


            foreach (var tool in _tools)
            {
                _compilation.RegisterTool(tool.Key, tool.Value);
            }

            _compilation.AddCSharpFile("console", consoleTree);
            _compilation.AddCSharpFile("random", randomTree);
        }

        public bool Busy()
        {
            return _busy;
        }


        public IEnumerable<Error> Compile()
        {
            if (_busy)
            {
                throw new InvalidOperationException();
            }

            _busy = true;
            IEnumerable<Error> errors = null;
            try
            {
                errors = DoCompile();
            }
            catch (Exception e)
            {
                errors = new[]
                {
                    new Error
                    {
                        File = "compiler",
                        Line = -1,
                        Message = e.Message
                    }
                };
            }

            _busy = false;
            return errors;
        }

        public IEnumerable<Error> Run(INotifier notifier, out dynamic client)
        {
            client = null;
            if (_busy)
            {
                throw new InvalidOperationException();
            }

            _notifier = notifier;
            _busy = true;
            IEnumerable<Error> errors = null;
            try
            {
                errors = DoCompile();
                if (errors == null)
                {
                    var assembly = _compilation.Build();
                    if (assembly != null)
                    {
                        SetupConsole(assembly);
                        DoRun(assembly, out client);
                    }
                    else
                    {
                        errors = GatherErrors(_compilation.Errors());
                    }
                }
            }
            catch (Exception e)
            {
                errors = new[]
                {
                    new Error
                    {
                        File = "compiler",
                        Line = -1,
                        Message = e.Message
                    }
                };
            }

            _busy = false;
            return errors;
        }

        public void Add(string file, int fileId, string contents)
        {
            if (_files.ContainsKey(file))
            {
                throw new InvalidOperationException("duplicate file");
            }

            _compilation.AddDocument(file, contents, GetInjector(file));

            _files[file] = fileId;
            _dirty = true;
        }

        public void Modify(string file, string contents)
        {
            if (!_files.ContainsKey(file))
            {
                throw new InvalidOperationException();
            }

            _compilation.UpdateDocument(file, contents);
            _dirty = true;
        }

        public abstract string DefaultFile();

        public string FileContents(string file)
        {
            if (_files.ContainsKey(file))
            {
                return _compilation.DocumentText(file);
            }

            return null;
        }

        public int FileId(string file)
        {
            int result;
            if (_files.TryGetValue(file, out result))
            {
                return result;
            }

            return -1;
        }

        public virtual IEnumerable<TreeNodeAction> FileActions(string file)
        {
            return new TreeNodeAction[] {};
        }

        public void setFilePath(dynamic path)
        {
            _compilation.SetPath(path);
        }

        private IEnumerable<Error> GatherErrors(IEnumerable<Diagnostic> diagnostics)
        {
            if (diagnostics == null)
            {
                return null;
            }

            return diagnostics
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic =>
                {
                    var mapped = _compilation.OriginalPosition(diagnostic.Location);
                    return new Error
                    {
                        File = mapped.Path,
                        Line = mapped.StartLinePosition.Line,
                        Character = mapped.StartLinePosition.Character,
                        Message = diagnostic.GetMessage()
                    };
                });
        }

        protected virtual ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel> GetInjector(string file)
        {
            return XSLang.Create();
        }

        protected virtual IEnumerable<Error> DoCompile()
        {
            if (!_dirty)
            {
                Notify(NotificationKind.System, "Compilation up to date");
                return null;
            }

            var result = _compilation.Compile();
            if (_dirty)
            {
                _dirty = false;
            }

            if (result)
            {
                return null;
            }

            return GatherErrors(_compilation.Errors());
        }

        protected abstract void DoRun(Assembly asm, out dynamic clientData);

        protected void Notify(NotificationKind kind, string message)
        {
            _notifier?.notify(new Notification {Kind = kind, Message = message});
        }

        private void SetupConsole(Assembly assembly)
        {
            var console = assembly.GetType("console");
            var method = console.GetMethod("setup");
            method.Invoke(null, new[] {(Action<string>) ConsoleNotify});
        }

        private void ConsoleNotify(string message)
        {
            Notify(NotificationKind.Application, message);
        }
    }
}