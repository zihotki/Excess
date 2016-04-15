using System.Diagnostics;

namespace Excess.Compiler.Core
{
    public abstract class CompilerBase<TToken, TNode, TModel> : ICompiler<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        protected IDocument<TToken, TNode, TModel> Document;
        protected ICompilerEnvironment _environment;
        protected IInstanceAnalisys<TNode> _instance;
        protected ILexicalAnalysis<TToken, TNode, TModel> _lexical;
        protected Scope _scope;
        protected ISemanticAnalysis<TToken, TNode, TModel> _semantics;
        protected CompilerStage Stage = CompilerStage.Started;
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

        public Scope Scope => _scope;

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
            Debug.Assert(Document == null);
            Document = CreateDocument();

            Document.ApplyChanges(stage);
            return Document.HasErrors();
        }

        public bool CompileAll(string text)
        {
            return Compile(text, CompilerStage.Finished);
        }

        public bool Advance(CompilerStage stage)
        {
            Document.ApplyChanges(stage);
            return Document.HasErrors();
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
}