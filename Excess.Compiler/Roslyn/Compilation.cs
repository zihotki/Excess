﻿using System;
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
    public class ToolEventArgs
    {
        public ICompilationTool Tool { get; set; }
        public string Document { get; set; }
        public Dictionary<string, string> Result { get; set; }
    }

    public delegate void ToolEventHandler(object sender, ToolEventArgs e);

    public class Compilation
    {
        private CSharpCompilation _compilation;

        private readonly Dictionary<string, SyntaxTree> _csharpFiles = new Dictionary<string, SyntaxTree>();

        private readonly List<CompilationDocument> _documents = new List<CompilationDocument>();

        private readonly RoslynEnvironment _environment;

        private readonly Scope _scope = new Scope(null);

        private readonly Dictionary<string, ICompilationTool> _tools = new Dictionary<string, ICompilationTool>();

        private readonly Dictionary<string, SyntaxTree> _trees = new Dictionary<string, SyntaxTree>();

        public string OutputFile { get; set; }

        public ICompilerEnvironment Environment { get { return _environment; } }

        public Compilation(IPersistentStorage storage)
        {
            _environment = createEnvironment(storage);
            _scope.set<ICompilerEnvironment>(_environment);
        }

        public void setPath(dynamic path)
        {
            _environment.SetPath(path);
        }

        public event ToolEventHandler ToolStarted;
        public event ToolEventHandler ToolFinished;

        public void registerTool(string ext, ICompilationTool tool)
        {
            if (_tools.ContainsKey(ext))
                throw new InvalidOperationException("Duplicate tool");

            _tools[ext] = tool;
        }

        public bool hasDocument(string id)
        {
            return _documents
                .Where(doc => doc.Id == id)
                .Any();
        }

        public void addDocument(string id, IDocument<SyntaxToken, SyntaxNode, SemanticModel> document)
        {
            if (_documents
                .Where(doc => doc.Id == id)
                .Any())
                throw new InvalidOperationException();

            var newDoc = new CompilationDocument
            {
                Id = id,
                Stage = CompilerStage.Started,
                Document = document
            };

            _documents.Add(newDoc);
        }

        public void addDocument(string id, string contents, ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel> injector)
        {
            if (_documents
                .Where(doc => doc.Id == id)
                .Any())
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
                addCSharpFile(id, contents);
            }
            else
            {
                if (_tools.TryGetValue(ext, out tool))
                {
                    var storage = _environment.Storage();
                    hash = storage == null
                        ? hash
                        : storage.CachedId(id);
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

        public void updateDocument(string id, string contents)
        {
            var doc = _documents
                .Where(document => document.Id == id)
                .FirstOrDefault();

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
                doc.Contents = contents;
        }

        public FileLinePositionSpan OriginalPosition(Location location)
        {
            var tree = location.SourceTree;
            if (tree == null)
                return default(FileLinePositionSpan);

            var document = documentByTree(tree);
            if (document == null)
                return default(FileLinePositionSpan);

            return document.OriginalPosition(location);
        }

        private RoslynDocument documentByTree(SyntaxTree tree)
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
            return doc != null
                ? doc.Document as RoslynDocument
                : null;
        }

        public string getFileByExtension(string ext)
        {
            var file = _documents
                .Where(document => Path.GetExtension(document.Id) == ext)
                .FirstOrDefault();

            if (file == null)
                return null;

            return file.Id;
        }

        public SyntaxTree getCSharpFile(string fileName)
        {
            SyntaxTree result;
            if (_csharpFiles.TryGetValue(fileName, out result))
                return result;

            return null;
        }

        public void addCSharpFile(string file, SyntaxTree tree)
        {
            var existing = _csharpFiles.ContainsKey(file);
            if (_compilation != null)
            {
                if (existing)
                    _compilation = _compilation.ReplaceSyntaxTree(_csharpFiles[file], tree);
                else
                    _compilation = _compilation.AddSyntaxTrees(tree);
            }

            _csharpFiles[file] = tree;
        }

        public void addCSharpFile(string file, string contents)
        {
            var tree = CSharp.ParseSyntaxTree(contents);
            addCSharpFile(file, tree);
        }

        public string documentText(string id)
        {
            var file = _documents
                .Where(document => document.Id == id)
                .FirstOrDefault();

            if (file != null)
            {
                if (file.Document != null)
                    return file.Document.Text;

                return file.Contents;
            }

            return null;
        }

        public bool compile()
        {
            bool changed;
            return doCompile(out changed);
        }

        private bool doCompile(out bool changed)
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
                    if (ToolStarted != null)
                        ToolStarted(this, new ToolEventArgs {Tool = tool, Document = doc.Id});

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

                    if (ToolFinished != null)
                        ToolFinished(this, new ToolEventArgs {Tool = tool, Document = doc.Id, Result = result});

                    if (!failed)
                    {
                        doc.Hash = hash;
                        toolResults(result, doc.Id, hash);
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
                        if (oldRoot == null)
                            _compilation = _compilation.AddSyntaxTrees(newTree);
                        else
                            _compilation = _compilation.ReplaceSyntaxTree(oldRoot.SyntaxTree, newTree);
                    }

                    _trees[doc.Id] = newTree;
                }

                if (document.HasErrors())
                    return false;
            }

            return true;
        }

        private void toolResults(Dictionary<string, string> result, string fileName, int hash)
        {
            var storage = _environment.Storage();
            if (storage != null)
                storage.CachedId(fileName, hash);

            foreach (var file in result)
            {
                if (hasDocument(file.Key))
                    updateDocument(file.Key, file.Value);
                else if (_csharpFiles.ContainsKey(file.Key))
                    addCSharpFile(file.Key, file.Value);
                else
                {
                    if (storage != null)
                        storage.AddFile(file.Key, file.Value, true);

                    addCSharpFile(file.Key, file.Value);
                }
            }
        }

        public Assembly build()
        {
            bool needsProcessing;
            if (!doCompile(out needsProcessing))
                return null;

            if (_compilation == null)
                _compilation = createCompilation();

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

            if (_compilation
                .GetDiagnostics()
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Any())
                return null;

            using (var stream = new MemoryStream())
            {
                var result = _compilation.Emit(stream);
                if (!result.Success)
                    return null;

                return Assembly.Load(stream.GetBuffer());
            }
        }

        private CSharpCompilation createCompilation()
        {
            var assemblyName = OutputFile;
            if (assemblyName == null)
                assemblyName = Guid.NewGuid().ToString().Replace("-", "");

            return CSharpCompilation.Create(assemblyName,
                _trees.Values
                    .Union(_csharpFiles.Values),
                _environment.GetReferences(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        protected virtual RoslynEnvironment createEnvironment(IPersistentStorage storage)
        {
            var result = new RoslynEnvironment(_scope, storage);
            result.Dependency<object>(new[] {"System", "System.Collections"});
            result.Dependency<Queue<object>>(new[] {"System.Collections.Generic"});
            result.Dependency<Expression>(new[] {"System.Linq"});

            return result;
        }

        public IEnumerable<Diagnostic> errors()
        {
            var diagnostics = null as IEnumerable<Diagnostic>;
            if (_compilation == null)
                _compilation = createCompilation();

            diagnostics = _compilation
                .GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error);

            var byFile = new Dictionary<string, List<Diagnostic>>();
            foreach (var diagnostic in diagnostics)
            {
                var tree = diagnostic.Location.SourceTree;
                var file = treeFile(tree);

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

        private string treeFile(SyntaxTree tree)
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