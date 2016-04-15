namespace Excess.Compiler
{
    public interface ICompilerInjector<TToken, TNode, TModel>
    {
        void Apply(ICompiler<TToken, TNode, TModel> compiler);
    }
}