using System.Diagnostics;

namespace Excess.Compiler.Core
{
    public abstract class CompilerBase<TToken, TNode, TModel> : ICompiler<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        protected IDocument<TToken, TNode, TModel> Document;
        protected CompilerStage Stage = CompilerStage.Started;

        public Scope Scope { get; protected set; }
        public ILexicalAnalysis<TToken, TNode, TModel> Lexical { get; protected set; }
        public ISyntaxAnalysis<TToken, TNode, TModel> Syntax { get; protected set; }
        public ISemanticAnalysis<TToken, TNode, TModel> Semantics { get; protected set; }
        public IInstanceAnalisys<TNode> Instance { get; protected set; }
        public ICompilerEnvironment Environment { get; protected set; }


        public CompilerBase(ILexicalAnalysis<TToken, TNode, TModel> lexical,
            ISyntaxAnalysis<TToken, TNode, TModel> syntax,
            ISemanticAnalysis<TToken, TNode, TModel> semantics,
            ICompilerEnvironment environment,
            IInstanceAnalisys<TNode> instance,
            Scope scope)
        {
            Lexical = lexical;
            Syntax = syntax;
            Semantics = semantics;
            Environment = environment;
            Instance = instance;

            Scope = new Scope(scope);
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
            var iLexical = Lexical as IDocumentInjector<TToken, TNode, TModel>;
            iLexical?.Apply(document);

            var iSyntax = Syntax as IDocumentInjector<TToken, TNode, TModel>;
            iSyntax?.Apply(document);

            var iSemantics = Semantics as IDocumentInjector<TToken, TNode, TModel>;
            iSemantics?.Apply(document);

            if (document is IInstanceDocument<TNode>)
            {
                var iInstance = Instance as IDocumentInjector<TToken, TNode, TModel>;
                iInstance?.Apply(document);
            }

            var iEnvironment = Environment as IDocumentInjector<TToken, TNode, TModel>;
            iEnvironment?.Apply(document);
        }

        protected abstract IDocument<TToken, TNode, TModel> CreateDocument();
    }
}