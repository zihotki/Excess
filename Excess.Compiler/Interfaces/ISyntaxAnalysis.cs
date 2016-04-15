using System;

namespace Excess.Compiler
{
    public interface ISyntaxAnalysis<TToken, TNode, TModel>
    {
        ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, Scope, SyntacticalExtensionDto<TNode>, TNode> handler);
        ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, SyntacticalExtensionDto<TNode>, TNode> handler);

        ISyntacticalMatch<TToken, TNode, TModel> Match(Func<TNode, bool> selector);
        ISyntacticalMatch<TToken, TNode, TModel> Match<T>(Func<T, bool> selector) where T : TNode;
        ISyntacticalMatch<TToken, TNode, TModel> Match<T>() where T : TNode;

        ISyntaxTransform<TNode> Transform();
        ISyntaxTransform<TNode> Transform(Func<TNode, Scope, TNode> handler);
        ISyntaxTransform<TNode> Transform(Func<TNode, TNode> handler);
    }
}