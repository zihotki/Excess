using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IGrammar<TToken, TNode, GNode>
    {
        GNode Parse(IEnumerable<TToken> tokens, Scope scope, int offset);
    }

    public interface IGrammarAnalysis<TGrammar, GNode, TToken, TNode>
    {
        IGrammarAnalysis<TGrammar, GNode, TToken, TNode> Transform<T>(Func<T, Func<GNode, Scope, TNode>, Scope, TNode> handler) where T : GNode;

        void Then(Func<TNode, TNode, Scope, LexicalExtension<TToken>, TNode> handler);
    }
}