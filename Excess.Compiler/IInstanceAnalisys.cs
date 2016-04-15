using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IInstanceAnalisys<TNode>
    {
        IInstanceMatch<TNode> match(Func<string, object, Scope, bool> handler);
        IInstanceMatch<TNode> match<Model>();
        IInstanceMatch<TNode> match(string id);
        void then(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform);
    }
}