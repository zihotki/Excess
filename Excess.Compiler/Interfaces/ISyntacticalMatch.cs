using System;

namespace Excess.Compiler
{
    public interface ISyntacticalMatch<TToken, TNode, TModel>
    {
        ISyntacticalMatch<TToken, TNode, TModel> Children(Func<TNode, bool> handler, string named = null);
        ISyntacticalMatch<TToken, TNode, TModel> Children<T>(Func<T, bool> handler = null, string named = null) where T : TNode;
        ISyntacticalMatch<TToken, TNode, TModel> Descendants(Func<TNode, bool> handler, string named = null);
        ISyntacticalMatch<TToken, TNode, TModel> Descendants<T>(Func<T, bool> handler = null, string named = null) where T : TNode;

        ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode> handler);
        ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler);
        ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler);
        ISyntaxAnalysis<TToken, TNode, TModel> Then(ISyntaxTransform<TNode> transform);
    }
}