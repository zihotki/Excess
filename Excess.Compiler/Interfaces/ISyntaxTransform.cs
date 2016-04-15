using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface ISyntaxTransform<TNode>
    {
        ISyntaxTransform<TNode> Remove(string nodes);
        ISyntaxTransform<TNode> Remove(Func<TNode, Scope, IEnumerable<TNode>> handler);
        ISyntaxTransform<TNode> Replace(string nodes, Func<TNode, Scope, TNode> handler);
        ISyntaxTransform<TNode> Replace(string nodes, Func<TNode, TNode> handler);
        ISyntaxTransform<TNode> Replace(Func<TNode, Scope, IEnumerable<TNode>> selector, Func<TNode, Scope, TNode> handler);
        ISyntaxTransform<TNode> Replace(Func<TNode, Scope, IEnumerable<TNode>> selector, Func<TNode, TNode> handler);
        ISyntaxTransform<TNode> AddToScope(string nodes, bool type = false, bool @namespace = false);
        ISyntaxTransform<TNode> AddToScope(Func<TNode, Scope, IEnumerable<TNode>> handler, bool type = false, bool @namespace = false);

        TNode Transform(TNode node, Scope scope);
    }
}