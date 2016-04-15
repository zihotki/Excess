using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Excess.Compiler.Core
{
    public class InstanceAnalisysBase<TToken, TNode, TModel> : IInstanceAnalisys<TNode>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        private readonly List<InstanceMatchBase<TNode>> _matchers = new List<InstanceMatchBase<TNode>>();

        private Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> _transform;

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            var doc = document as IInstanceDocument<TNode>;

            Debug.Assert(doc != null);
            Debug.Assert(_transform != null);
            doc.Change(_transform);

            foreach (var matcher in _matchers)
            {
                matcher.Apply(doc);
            }
        }

        public IInstanceMatch<TNode> Match(Func<string, object, Scope, bool> handler)
        {
            var result = new InstanceMatchBase<TNode>(this, handler);
            _matchers.Add(result);
            return result;
        }

        public IInstanceMatch<TNode> Match<TMatchModel>()
        {
            return Match((id, instance, scope) => instance is TMatchModel);
        }

        public IInstanceMatch<TNode> Match(string id)
        {
            return Match((iid, instance, scope) => iid == id);
        }

        public void Then(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform)
        {
            Debug.Assert(_transform == null);
            _transform = transform;
        }
    }
}