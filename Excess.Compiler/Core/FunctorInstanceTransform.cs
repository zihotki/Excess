using System;
using System.Collections.Generic;

namespace Excess.Compiler.Core
{
    public class FunctorInstanceTransform<TNode> : InstanceTransformBase<TNode>
    {
        private readonly Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> _handler;

        public FunctorInstanceTransform(
            Dictionary<string, InstanceConnector> input,
            Dictionary<string, InstanceConnector> output,
            Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>> connectorDataTransform,
            Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>> connectionTransform,
            Action<InstanceConnection<TNode>, Scope> defaultInputTransform,
            Action<InstanceConnection<TNode>, Scope> defaultOutputTransform,
            Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler)
            : base(input, output, connectorDataTransform, connectionTransform, defaultInputTransform, defaultOutputTransform)
        {
            _handler = handler;
        }

        protected override TNode DoTransform(string id, object instance, TNode node, IEnumerable<InstanceConnection<TNode>> connections, Scope scope)
        {
            return _handler(id, instance, node, connections, scope);
        }
    }
}