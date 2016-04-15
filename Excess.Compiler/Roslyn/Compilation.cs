using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Excess.Compiler.Roslyn
{
    public class Compilation
    {
        private readonly Dictionary<string, SyntaxTree> _csharpFiles = new Dictionary<string, SyntaxTree>();

        private readonly List<CompilationDocument> _documents = new List<CompilationDocument>();

        private readonly RoslynEnvironment _environment;

        private readonly Scope _scope = new Scope(null);

        private readonly Dictionary<string, ICompilationTool> _tools = new Dictionary<string, ICompilationTool>();

        private readonly Dictionary<string, SyntaxTree> _trees = new Dictionary<string, SyntaxTree>();
        private CSharpCompilation _compilation;

        public string OutputFile { get; set; }

        public ICompilerEnvironment Environment => _environment;

        public Compilation(IPersistentStorage storage)
        {
            _environment = CreateEnvironment(storage);
            _scope.Set<ICompilerEnvironment>(_environment);
        }

        public void SetPath(dynamic path)
        {
            _environment.SetPath(path);
        }

        public event ToolEventHandler ToolStarted;
        public event ToolEventHandler ToolFinished;

        public void RegisterTool(string ext, ICompilationTool tool)
        {
            if (_tools.ContainsKey(ext))
                throw new InvalidOperationException("Duplicate tool");

            _tools[ext] = tool;
        }

        public bool HasDocument(string id)
        {
            return _documents.Any(doc => doc.Id == id);
        }

        public void AddDocument(string id, IDocument<SyntaxToken, SyntaxNode, SemanticModel> document)
        {
            if (_documents.Any(doc => doc.Id == id))
                throw new InvalidOperationException();

            var newDoc = new CompilationDocument
            {
                Id = id,
                Stage = CompilerStage.Started,
                Document = document
            };

            _documents.Add(newDoc);
        }

        public void AddDocument(string id, string contents, ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel> injector)
        {
            if (_documents.Any(doc => doc.Id == id))
                throw new InvalidOperationException();

            var ext = Path.GetExtension(id);
            var compiler = null as RoslynCompiler;
            var tool = null as ICompilationTool;
            var hash = 0;
            if (string.IsNullOrEmpty(ext))
            {
                compiler = new RoslynCompiler(_environment, _scope);
                injector.Apply(compiler);
            }
            else if (ext == ".cs")
            {
                AddCSharpFile(id, contents);
            }
            else
            {
                if (_tools.TryGetValue(ext, out tool))
                {
                    var storage = _environment.Storage();
                    hash = storage?.CachedId(id) ?? hash;
                }
            }

            var newDoc = new CompilationDocument
            {
                Id = id,
                Stage = CompilerStage.Started,
                Compiler = compiler,
                Tool = tool,
                Hash = hash
            };

            if (compiler != null)
            {
                newDoc.Stage = CompilerStage.Started;
                newDoc.Document = new RoslynDocument(compiler.Scope, contents, id);

                var documentInjector = newDoc.Compiler as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
                documentInjector.Apply(newDoc.Document);
            }
            else
                newDoc.Contents = contents;

            _documents.Add(newDoc);
        }

        public void UpdateDocument(string id, string contents)
        {
            var doc = _documents.FirstOrDefault(document => document.Id == id);

            if (doc == null)
                throw new InvalidOperationException();

            if (doc.Document != null)
            {
                if (_compilation != null && doc.Document.SyntaxRoot != null)
                    _compilation = _compilation.RemoveSyntaxTrees(doc.Document.SyntaxRoot.SyntaxTree);

                doc.Document = new RoslynDocument(doc.Compiler.Scope, contents, id);

                var documentInjector = doc.Compiler as IDocumentInjector<SyntaxToken, SyntaxNode, SemanticModel>;
                documentInjector.Apply(doc.Document);

                doc.Stage = CompilerStage.Started;
            }
            else
            {
                doc.Contents = contents;
            }
        }

        public FileLinePositionSpan OriginalPosition(Location location)
        {
            var tree = location.SourceTree;
            if (tree == null)
                return default(FileLinePositionSpan);

            var document = DocumentByTree(tree);
            if (document == null)
                return default(FileLinePositionSpan);

            return document.OriginalPosition(location);
        }

        private RoslynDocument DocumentByTree(SyntaxTree tree)
        {
            string id = null;
            foreach (var cTree in _trees)
            {
                if (cTree.Value == tree)
                {
                    id = cTree.Key;
                    break;
                }
            }

            if (id == null)
                return null;

            var doc = _documents.Find(document => document.Id == id);
            return doc?.Document as RoslynDocument;
        }

        public string GetFileByExtension(string ext)
        {
            var file = _documents.FirstOrDefault(document => Path.GetExtension(document.Id) == ext);

            return file?.Id;
        }

        public SyntaxTree GetCSharpFile(string fileName)
        {
            SyntaxTree result;
            if (_csharpFiles.TryGetValue(fileName, out result))
                return result;

            return null;
        }

        public void AddCSharpFile(string file, SyntaxTree tree)
        {
            var existing = _csharpFiles.ContainsKey(file);
            if (_compilation != null)
            {
                _compilation = existing
                    ? _compilation.ReplaceSyntaxTree(_csharpFiles[file], tree)
                    : _compilation.AddSyntaxTrees(tree);
            }

            _csharpFiles[file] = tree;
        }

        public void AddCSharpFile(string file, string contents)
        {
            var tree = CSharp.ParseSyntaxTree(contents);
            AddCSharpFile(file, tree);
        }

        public string DocumentText(string id)
        {
            var file = _documents.FirstOrDefault(document => document.Id == id);

            if (file != null)
            {
                if (file.Document != null)
                    return file.Document.Text;

                return file.Contents;
            }

            return null;
        }

        public bool Compile()
        {
            bool changed;
            return DoCompile(out changed);
        }

        private bool DoCompile(out bool changed)
        {
            changed = false;
            foreach (var doc in _documents)
            {
                var tool = doc.Tool;
                if (tool == null)
                    continue;

                var hash = 0;
                if (!tool.DoNotCache)
                    hash = doc.Contents.GetHashCode();

                if (hash == 0 || hash != doc.Hash)
                {
                    var result = new Dictionary<string, string>();
                    ToolStarted?.Invoke(this, new ToolEventArgs {Tool = tool, Document = doc.Id});

                    var failed = false;
                    try
                    {
                        tool.Compile(doc.Id, doc.Contents, _scope, result);
                    }
                    catch (Exception e)
                    {
                        failed = true;
                        result["error"] = e.Message;
                    }

                    ToolFinished?.Invoke(this, new ToolEventArgs { Tool = tool, Document = doc.Id, Result = result });

                    if (!failed)
                    {
                        doc.Hash = hash;
                        ToolResults(result, doc.Id, hash);
                    }
                }
            }

            foreach (var doc in _documents)
            {
                if (doc.Compiler == null)
                    continue;

                var document = doc.Document;
                if (document.Stage <= CompilerStage.Syntactical)
                {
                    changed = true;

                    var oldRoot = document.SyntaxRoot;
                    document.ApplyChanges(CompilerStage.Syntactical);
                    var newRoot = document.SyntaxRoot;

                    Debug.Assert(newRoot != null);
                    var newTree = newRoot.SyntaxTree;

                    if (_compilation != null)
                    {
                        _compilation = oldRoot == null
                            ? _compilation.AddSyntaxTrees(newTree)
                            : _compilation.ReplaceSyntaxTree(oldRoot.SyntaxTree, newTree);
                    }

                    _trees[doc.Id] = newTree;
                }

                if (document.HasErrors())
                    return false;
            }

            return true;
        }

        private void ToolResults(Dictionary<string, string> result, string fileName, int hash)
        {
            var storage = _environment.Storage();
            storage?.CachedId(fileName, hash);

            foreach (var file in result)
            {
                if (HasDocument(file.Key))
                    UpdateDocument(file.Key, file.Value);
                else if (_csharpFiles.ContainsKey(file.Key))
                    AddCSharpFile(file.Key, file.Value);
                else
                {
                    storage?.AddFile(file.Key, file.Value, true);

                    AddCSharpFile(file.Key, file.Value);
                }
            }
        }

        public Assembly Build()
        {
            bool needsProcessing;
            if (!DoCompile(out needsProcessing))
                return null;

            if (_compilation == null)
                _compilation = CreateCompilation();

            while (needsProcessing)
            {
                needsProcessing = false;
                foreach (var doc in _documents)
                {
                    var document = doc.Document;
                    if (document == null)
                        continue;

                    var tree = document.SyntaxRoot.SyntaxTree;

                    document.Model = _compilation.GetSemanticModel(tree);
                    Debug.Assert(document.Model != null);

                    var oldRoot = document.SyntaxRoot;
                    if (!document.ApplyChanges(CompilerStage.Semantical))
                        needsProcessing = true;

                    var newRoot = document.SyntaxRoot;


                    Debug.Assert(oldRoot != null && newRoot != null);
                    var newTree = newRoot.SyntaxTree;
                    if (oldRoot != newRoot)
                    {
                        _compilation = _compilation.ReplaceSyntaxTree(oldRoot.SyntaxTree, newTree);
                        _trees[doc.Id] = newTree;
                    }
                }
            }

            if (_compilation.GetDiagnostics().Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
                return null;

            using (var stream = new MemoryStream())
            {
                var result = _compilation.Emit(stream);
                if (!result.Success)
                    return null;

                return Assembly.Load(stream.GetBuffer());
            }
        }

        private CSharpCompilation CreateCompilation()
        {
            var assemblyName = OutputFile ?? Guid.NewGuid().ToString().Replace("-", "");

            return CSharpCompilation.Create(assemblyName,
                _trees.Values.Union(_csharpFiles.Values),
                _environment.GetReferences(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        protected virtual RoslynEnvironment CreateEnvironment(IPersistentStorage storage)
        {
            var result = new RoslynEnvironment(_scope, storage);
            result.Dependency<object>(new[] {"System", "System.Collections"});
            result.Dependency<Queue<object>>(new[] {"System.Collections.Generic"});
            result.Dependency<Expression>(new[] {"System.Linq"});

            return result;
        }

        public IEnumerable<Diagnostic> Errors()
        {
            if (_compilation == null)
                _compilation = CreateCompilation();

            var diagnostics = _compilation
                .GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error);

            var byFile = new Dictionary<string, List<Diagnostic>>();
            foreach (var diagnostic in diagnostics)
            {
                var tree = diagnostic.Location.SourceTree;
                var file = TreeFile(tree);

                if (file != null)
                {
                    List<Diagnostic> fileDiagnostics;
                    if (!byFile.TryGetValue(file, out fileDiagnostics))
                    {
                        fileDiagnostics = new List<Diagnostic>();
                        byFile[file] = fileDiagnostics;
                    }

                    fileDiagnostics.Add(diagnostic);
                }
                else
                    yield return diagnostic; //td: maybe ignore?
            }

            foreach (var doc in _documents)
            {
                var document = doc.Document as RoslynDocument;
                if (document == null)
                    continue;

                var errors = document.GetErrors();

                //native errors
                List<Diagnostic> fileDiagnostics;
                if (byFile.TryGetValue(doc.Id, out fileDiagnostics))
                {
                    if (errors == null)
                        errors = fileDiagnostics;
                    else
                        errors = errors.Union(fileDiagnostics);
                }

                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        yield return error;
                    }
                }
            }
        }

        private string TreeFile(SyntaxTree tree)
        {
            foreach (var file in _trees)
            {
                if (file.Value == tree)
                    return file.Key;
            }

            return null;
        }

        private class CompilationDocument
        {
            public string Id { get; set; }
            public CompilerStage Stage { get; set; }
            public IDocument<SyntaxToken, SyntaxNode, SemanticModel> Document { get; set; }
            public RoslynCompiler Compiler { get; set; }
            public ICompilationTool Tool { get; set; }
            public int Hash { get; set; }
            public string Contents { get; set; }
        }
    }
}