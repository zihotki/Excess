using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IGrammar<TToken, TNode, TGNode>
    {
        TGNode Parse(IEnumerable<TToken> tokens, Scope scope, int offset);
    }
}