namespace Excess.Compiler
{
    public interface IDocumentInjector<TToken, TNode, TModel>
    {
        void Apply(IDocument<TToken, TNode, TModel> document);
    }
}