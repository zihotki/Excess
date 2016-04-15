using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IInstanceAnalisys<TNode>
    {
        IInstanceMatch<TNode> Match(Func<string, object, Scope, bool> handler);
        IInstanceMatch<TNode> Match<TModel>();
        IInstanceMatch<TNode> Match(string id);
        void Then(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform);
    }
}