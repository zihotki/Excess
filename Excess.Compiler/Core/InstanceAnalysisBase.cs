using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excess.Compiler.Core
{
	public class FunctorInstanceTransform<TNode> : InstanceTransformBase<TNode>
    {
        Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> _handler;
        public FunctorInstanceTransform(
            Dictionary<string, InstanceConnector> input,
            Dictionary<string, InstanceConnector> output,
            Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>> connectorDataTransform,
            Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>> connectionTransform,
            Action<InstanceConnection<TNode>, Scope> defaultInputTransform,
            Action<InstanceConnection<TNode>, Scope> defaultOutputTransform,
            Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler) : base(input, output, connectorDataTransform, connectionTransform, defaultInputTransform, defaultOutputTransform)
        {
            _handler = handler;
        }

        protected override TNode DoTransform(string id, object instance, TNode node, IEnumerable<InstanceConnection<TNode>> connections, Scope scope)
        {
            return _handler(id, instance, node, connections, scope);
        }
    }

    public class InstanceDocumentBase<TNode> : IInstanceDocument<TNode>
    {
        public InstanceDocumentBase()
        {
        }

        Dictionary<Func<string, object, Scope, bool>, IInstanceTransform<TNode>> _mt = new Dictionary<Func<string, object, Scope, bool>, IInstanceTransform<TNode>>();
        public void Change(Func<string, object, Scope, bool> match, IInstanceTransform<TNode> transform)
        {
            _mt[match] = transform;
        }

        Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> _transform;
        public void Change(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform)
        {
            Debug.Assert(_transform == null);
            _transform = transform;
        }

        bool _hasErrors = false;
        public TNode transform(IDictionary<string, object> instances, IEnumerable<Connection> connections, Scope scope)
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
                var source = null as Instance<TNode>;
                var target = null as Instance<TNode>;
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

                var outputDT = null as Action<InstanceConnector, object, object, Scope>;
                var outputTransform = null as Action<InstanceConnector, InstanceConnection<TNode>, Scope>;
                var output = source.Transform.Output(connection.OutputConnector, out outputDT, out outputTransform);
                if (output == null)
                    output = new InstanceConnector { Id = connection.OutputConnector };

                var inputDT = null as Action<InstanceConnector, object, object, Scope>;
                var inputTransform = null as Action<InstanceConnector, InstanceConnection<TNode>, Scope>;
                var input = target.Transform.Input(connection.InputConnector, out inputDT, out inputTransform);
                if (input == null)
                    input = new InstanceConnector { Id = connection.InputConnector };

                var iconn = new InstanceConnection<TNode>()
                {
                    Source = source.Id,
                    Output = output,
                    OutputModel = source.Value,
                    OutputModelNode = source.Node,
                    Target = target.Id,
                    Input = input,
                    InputModel = target.Value,
                    InputModelNode = target.Node,
                };

                if (outputDT != null)
                    outputDT(iconn.Output, iconn.InputModel, iconn.OutputModel, scope);

                if (inputDT != null)
                    inputDT(iconn.Input, iconn.OutputModel, iconn.InputModel, scope);

                iconn.InputTransform = inputTransform;
                iconn.OutputTransform = outputTransform;
                source.Connections.Add(iconn);
                target.Connections.Add(iconn);
            }

            foreach (var instance in namedInstances.Values)
            {
                transformInstance(instance, scope);
            }


            foreach (var instance in namedInstances.Values)
            {
                foreach (var conn in instance.Connections)
                {
                    bool isInput = instance.Id == conn.Target;
                    var inputInstance = instance;
                    var outputInstance = instance;
                    if (isInput)
                    {
                        outputInstance = namedInstances[conn.Target];
                        applyConnection(conn, outputInstance, isInput, conn.InputTransform, scope);
                    }
                    else
                    {
                        inputInstance = namedInstances[conn.Source];
                        applyConnection(conn, inputInstance, isInput, conn.OutputTransform, scope);
                    }
                }
            }

            if (_hasErrors)
                return default(TNode);

            var items = new Dictionary<string, Tuple<object, TNode>>();
            foreach (var item in namedInstances)
                items[item.Key] = new Tuple<object, TNode>(item.Value.Value, item.Value.Node);

            return _transform(items, scope);
        }

        private void transformInstance(Instance<TNode> instance, Scope scope)
        {
            instance.Node = instance.Transform.Transform(instance.Id, instance.Value, instance.Node, instance.Connections, scope);

            foreach (var conn in instance.Connections)
            {
                bool isInput = instance.Id == conn.Target;
                if (isInput)
                    conn.InputModelNode = instance.Node;
                else
                    conn.OutputModelNode = instance.Node;
            }
        }

        private void applyConnection(
            InstanceConnection<TNode> connection,
            Instance<TNode> instance,
            bool isInput,
            Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform, 
            Scope scope)
        {

            if (transform != null)
            {
                var connector = isInput ? connection.Input : connection.Output;

                transform(connector, connection, scope);
                instance.Node = isInput ? connection.InputModelNode : connection.OutputModelNode;
            }
        }
    }


    public class InstanceAnalisysBase<TToken, TNode, TModel> : IInstanceAnalisys<TNode>, 
                                                               IDocumentInjector<TToken, TNode, TModel>
    {
        List<InstanceMatchBase<TNode>> _matchers = new List<InstanceMatchBase<TNode>>();
        public IInstanceMatch<TNode> match(Func<string, object, Scope, bool> handler)
        {
            var result = new InstanceMatchBase<TNode>(this, handler);
            _matchers.Add(result);
            return result;
        }

        public IInstanceMatch<TNode> match<Model>()
        {
            return match((id, instance, scope) => instance is Model);
        }

        public IInstanceMatch<TNode> match(string id)
        {
            return match((iid, instance, scope) => iid == id);
        }

        Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> _transform;
        public void then(Func<IDictionary<string, Tuple<object, TNode>>, Scope, TNode> transform)
        {
            Debug.Assert(_transform == null);
            _transform = transform;
        }

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            var doc = document as IInstanceDocument<TNode>;

            Debug.Assert(doc != null);
            Debug.Assert(_transform != null);
            doc.Change(_transform);

            foreach (var matcher in _matchers)
                matcher.Apply(doc);
        }
    }

}

