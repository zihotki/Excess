using System;
using System.Collections.Generic;

namespace Excess.Compiler.Core
{
    public abstract class InstanceTransformBase<TNode> : IInstanceTransform<TNode>
    {
        private readonly Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>> _connectionTransform;
        private readonly Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>> _connectorDataTransform;
        private readonly Action<InstanceConnection<TNode>, Scope> _defaultInputTransform;
        private readonly Action<InstanceConnection<TNode>, Scope> _defaultOutputTransform;
        private readonly Dictionary<string, InstanceConnector> _input;
        private readonly Dictionary<string, InstanceConnector> _output;

        public InstanceTransformBase(
            Dictionary<string, InstanceConnector> input,
            Dictionary<string, InstanceConnector> output,
            Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>> connectorDataTransform,
            Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>> connectionTransform,
            Action<InstanceConnection<TNode>, Scope> defaultInputTransform,
            Action<InstanceConnection<TNode>, Scope> defaultOutputTransform)
        {
            _input = input;
            _output = output;
            _connectorDataTransform = connectorDataTransform;
            _connectionTransform = connectionTransform;
            _defaultInputTransform = defaultInputTransform;
            _defaultOutputTransform = defaultOutputTransform;
        }

        public TNode Transform(string id, object instance, TNode node, IEnumerable<InstanceConnection<TNode>> connections, Scope scope)
        {
            var result = DoTransform(id, instance, node, connections, scope);
            foreach (var connection in connections)
            {
                var isInput = connection.InputModel == instance;
                if (isInput)
                {
                    var transform = connection.InputTransform;
                    var found = false;
                    if (connection.Input != null)
                    {
                        found = _connectionTransform.TryGetValue(connection.Input, out transform);
                    }

                    if (!found && _defaultInputTransform != null)
                    {
                        transform = (connector, connection_, scope_) => _defaultInputTransform(connection_, scope_);
                    }

                    connection.InputTransform = transform;
                }
                else
                {
                    var transform = connection.InputTransform;
                    var found = false;
                    if (connection.Output != null)
                    {
                        found = _connectionTransform.TryGetValue(connection.Output, out transform);
                    }

                    if (!found && _defaultOutputTransform != null)
                    {
                        transform = (connector, connection_, scope_) => _defaultOutputTransform(connection_, scope_);
                    }

                    connection.InputTransform = transform;
                }
            }

            return result;
        }

        public InstanceConnector Output(string connector,
            out Action<InstanceConnector, object, object, Scope> dt,
            out Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform)
        {
            dt = null;
            transform = null;

            var result = null as InstanceConnector;
            if (_output.TryGetValue(connector, out result))
            {
                _connectorDataTransform.TryGetValue(result, out dt);
                _connectionTransform.TryGetValue(result, out transform);
            }

            return result;
        }

        public InstanceConnector Input(string connector,
            out Action<InstanceConnector, object, object, Scope> dt,
            out Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform)
        {
            dt = null;
            transform = null;

            var result = null as InstanceConnector;
            if (_input.TryGetValue(connector, out result))
            {
                _connectorDataTransform.TryGetValue(result, out dt);
                _connectionTransform.TryGetValue(result, out transform);
            }

            return result;
        }

        protected abstract TNode DoTransform(string id, object instance, TNode node, IEnumerable<InstanceConnection<TNode>> connections, Scope scope);
    }
}