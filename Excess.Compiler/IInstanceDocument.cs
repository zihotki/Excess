using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IInstanceDocument<TNode>
    {
        void Change(Func<string, object, Scope, bool> match, IInstanceTransform<TNode> transform);
        void Change(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform);
    }
}