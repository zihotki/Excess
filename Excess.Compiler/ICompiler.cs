namespace Excess.Compiler
{
    public interface ICompiler<TToken, TNode, TModel>
    {
        Scope Scope { get; }
        ILexicalAnalysis<TToken, TNode, TModel> Lexical();
        ISyntaxAnalysis<TToken, TNode, TModel> Syntax();
        ISemanticAnalysis<TToken, TNode, TModel> Semantics();
        IInstanceAnalisys<TNode> Instance();
        ICompilerEnvironment Environment();

        bool Compile(string text, CompilerStage stage = CompilerStage.Started);
        bool CompileAll(string text);
        bool Advance(CompilerStage stage);
    }
}