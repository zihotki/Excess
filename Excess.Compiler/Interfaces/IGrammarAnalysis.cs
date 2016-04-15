using System;

namespace Excess.Compiler
{
    public interface IGrammarAnalysis<TGrammar, TGNode, TToken, TNode>
    {
        IGrammarAnalysis<TGrammar, TGNode, TToken, TNode> Transform<T>(Func<T, Func<TGNode, Scope, TNode>, Scope, TNode> handler) where T : TGNode;

        void Then(Func<TNode, TNode, Scope, LexicalExtensionDto<TToken>, TNode> handler);
    }
}