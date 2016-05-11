namespace Excess.Compiler
{
    public interface ICompiler<TToken, TNode, TModel>
    {
        Scope Scope { get; }
        IInstanceAnalisys<TNode> Instance { get; }
        ICompilerEnvironment Environment { get; }

        ILexicalAnalysis<TToken, TNode, TModel> Lexical { get; }
        ISyntaxAnalysis<TToken, TNode, TModel> Syntax { get; }
        ISemanticAnalysis<TToken, TNode, TModel> Semantics { get; }

        bool Compile(string text, CompilerStage stage = CompilerStage.Started);
        bool CompileAll(string text);
        bool Advance(CompilerStage stage);
    }
}