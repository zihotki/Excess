using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Excess.Compiler.Core
{
    public abstract class CompilerBase<TToken, TNode, TModel> : ICompiler<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        protected IDocument<TToken, TNode, TModel> _document;
        protected ICompilerEnvironment _environment;
        protected IInstanceAnalisys<TNode> _instance;
        protected ILexicalAnalysis<TToken, TNode, TModel> _lexical;
        protected Scope _scope;
        protected ISemanticAnalysis<TToken, TNode, TModel> _semantics;
        protected CompilerStage _stage = CompilerStage.Started;
        protected ISyntaxAnalysis<TToken, TNode, TModel> _syntax;

        public CompilerBase(ILexicalAnalysis<TToken, TNode, TModel> lexical,
            ISyntaxAnalysis<TToken, TNode, TModel> syntax,
            ISemanticAnalysis<TToken, TNode, TModel> semantics,
            ICompilerEnvironment environment,
            IInstanceAnalisys<TNode> instance,
            Scope scope)
        {
            _lexical = lexical;
            _syntax = syntax;
            _semantics = semantics;
            _environment = environment;
            _instance = instance;

            _scope = new Scope(scope);
        }

        public Scope Scope { get { return _scope; } }

        public ILexicalAnalysis<TToken, TNode, TModel> Lexical()
        {
            return _lexical;
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Syntax()
        {
            return _syntax;
        }

        public ISemanticAnalysis<TToken, TNode, TModel> Semantics()
        {
            return _semantics;
        }

        public IInstanceAnalisys<TNode> Instance()
        {
            return _instance;
        }

        public ICompilerEnvironment Environment()
        {
            return _environment;
        }

        public bool Compile(string text, CompilerStage stage)
        {
            Debug.Assert(_document == null);
            _document = CreateDocument();

            _document.ApplyChanges(stage);
            return _document.HasErrors();
        }

        public bool CompileAll(string text)
        {
            return Compile(text, CompilerStage.Finished);
        }

        public bool Advance(CompilerStage stage)
        {
            _document.ApplyChanges(stage);
            return _document.HasErrors();
        }

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            var iLexical = _lexical as IDocumentInjector<TToken, TNode, TModel>;
            iLexical?.Apply(document);

            var iSyntax = _syntax as IDocumentInjector<TToken, TNode, TModel>;
            iSyntax?.Apply(document);

            var iSemantics = _semantics as IDocumentInjector<TToken, TNode, TModel>;
            iSemantics?.Apply(document);

            if (document is IInstanceDocument<TNode>)
            {
                var iInstance = _instance as IDocumentInjector<TToken, TNode, TModel>;
                iInstance?.Apply(document);
            }

            var iEnvironment = _environment as IDocumentInjector<TToken, TNode, TModel>;
            iEnvironment?.Apply(document);
        }

        protected abstract IDocument<TToken, TNode, TModel> CreateDocument();
    }

    public class DelegateInjector<TToken, TNode, TModel> : ICompilerInjector<TToken, TNode, TModel>
    {
        private readonly Action<ICompiler<TToken, TNode, TModel>> _delegate;

        public DelegateInjector(Action<ICompiler<TToken, TNode, TModel>> @delegate)
        {
            _delegate = @delegate;
        }

        public void Apply(ICompiler<TToken, TNode, TModel> compiler)
        {
            _delegate(compiler);
        }
    }

    public class CompositeInjector<TToken, TNode, TModel> : ICompilerInjector<TToken, TNode, TModel>
    {
        private readonly IEnumerable<ICompilerInjector<TToken, TNode, TModel>> _injectors;

        public CompositeInjector(IEnumerable<ICompilerInjector<TToken, TNode, TModel>> injectors)
        {
            _injectors = injectors;
        }

        public void Apply(ICompiler<TToken, TNode, TModel> compiler)
        {
            foreach (var injector in _injectors)
            {
                injector.Apply(compiler);
            }
        }
    }
}