using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Excess.Compiler.Core
{
    public class InstanceDocumentBase<TNode> : IInstanceDocument<TNode>
    {
        private readonly bool _hasErrors = false;

        private readonly Dictionary<Func<string, object, Scope, bool>, IInstanceTransform<TNode>> _mt =
            new Dictionary<Func<string, object, Scope, bool>, IInstanceTransform<TNode>>();

        private Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> _transform;

        public void Change(Func<string, object, Scope, bool> match, IInstanceTransform<TNode> transform)
        {
            _mt[match] = transform;
        }

        public void Change(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform)
        {
            Debug.Assert(_transform == null);
            _transform = transform;
        }

        public TNode Transform(IDictionary<string, object> instances, IEnumerable<Connection> connections, Scope scope)
        {
            Debug.Assert(_transform != null);

            //match
            var namedInstances = new Dictionary<string, Instance<TNode>>();
            foreach (var i in instances)
            {
                Debug.Assert(i.Value != null);

                foreach (var mt in _mt)
                {
                    var match = mt.Key;
                    var transform = mt.Value;

                    if (match(i.Key, i.Value, scope))
                    {
                        var instance = new Instance<TNode>
                        {
                            Id = i.Key,
                            Value = i.Value
                        };

                        namedInstances[instance.Id] = instance;

                        Debug.Assert(instance.Transform == null);
                        instance.Transform = transform;
                    }
                }
            }

            //apply connections, save 
            foreach (var connection in connections)
            {
                Instance<TNode> source;
                Instance<TNode> target;
                if (!namedInstances.TryGetValue(connection.Source, out source))
                {
                    //td: error, invalid source
                    continue;
                }

                if (!namedInstances.TryGetValue(connection.Target, out target))
                {
                    //td: error, invalid source
                    continue;
                }

                Action<InstanceConnector, object, object, Scope> outputDt;
                Action<InstanceConnector, InstanceConnection<TNode>, Scope> outputTransform;
                var output = source.Transform.Output(connection.OutputConnector, out outputDt, out outputTransform)
                             ?? new InstanceConnector {Id = connection.OutputConnector};

                Action<InstanceConnector, object, object, Scope> inputDt;
                Action<InstanceConnector, InstanceConnection<TNode>, Scope> inputTransform;
                var input = target.Transform.Input(connection.InputConnector, out inputDt, out inputTransform)
                            ?? new InstanceConnector {Id = connection.InputConnector};

                var iconn = new InstanceConnection<TNode>
                {
                    Source = source.Id,
                    Output = output,
                    OutputModel = source.Value,
                    OutputModelNode = source.Node,
                    Target = target.Id,
                    Input = input,
                    InputModel = target.Value,
                    InputModelNode = target.Node
                };

                outputDt?.Invoke(iconn.Output, iconn.InputModel, iconn.OutputModel, scope);

                inputDt?.Invoke(iconn.Input, iconn.OutputModel, iconn.InputModel, scope);

                iconn.InputTransform = inputTransform;
                iconn.OutputTransform = outputTransform;
                source.Connections.Add(iconn);
                target.Connections.Add(iconn);
            }

            foreach (var instance in namedInstances.Values)
            {
                TransformInstance(instance, scope);
            }


            foreach (var instance in namedInstances.Values)
            {
                foreach (var conn in instance.Connections)
                {
                    var isInput = instance.Id == conn.Target;
                    if (isInput)
                    {
                        var outputInstance = namedInstances[conn.Target];
                        ApplyConnection(conn, outputInstance, isInput, conn.InputTransform, scope);
                    }
                    else
                    {
                        var inputInstance = namedInstances[conn.Source];
                        ApplyConnection(conn, inputInstance, isInput, conn.OutputTransform, scope);
                    }
                }
            }

            if (_hasErrors)
            {
                return default(TNode);
            }

            var items = new Dictionary<string, Tuple<object, TNode>>();
            foreach (var item in namedInstances)
            {
                items[item.Key] = new Tuple<object, TNode>(item.Value.Value, item.Value.Node);
            }

            return _transform(items, scope);
        }

        private void TransformInstance(Instance<TNode> instance, Scope scope)
        {
            instance.Node = instance.Transform.Transform(instance.Id, instance.Value, instance.Node, instance.Connections, scope);

            foreach (var conn in instance.Connections)
            {
                var isInput = instance.Id == conn.Target;
                if (isInput)
                    conn.InputModelNode = instance.Node;
                else
                    conn.OutputModelNode = instance.Node;
            }
        }

        private void ApplyConnection(
            InstanceConnection<TNode> connection,
            Instance<TNode> instance,
            bool isInput,
            Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform,
            Scope scope)
        {
            if (transform != null)
            {
                var connector = isInput
                    ? connection.Input
                    : connection.Output;

                transform(connector, connection, scope);
                instance.Node = isInput
                    ? connection.InputModelNode
                    : connection.OutputModelNode;
            }
        }
    }
}