using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Excess.Compiler.Core
{
	public class InstanceMatchBase<TNode> : IInstanceMatch<TNode>
	{
		IInstanceAnalisys<TNode> _owner;
		Func<string, object, Scope, bool> _match;


		Dictionary<string, InstanceConnector> _input = new Dictionary<string, InstanceConnector>();
		Dictionary<string, InstanceConnector> _output = new Dictionary<string, InstanceConnector>();
		Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>> _dataTransform = new Dictionary<InstanceConnector, Action<InstanceConnector, object, object, Scope>>();
		Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>> _nodeTransform = new Dictionary<InstanceConnector, Action<InstanceConnector, InstanceConnection<TNode>, Scope>>();
		Action<InstanceConnection<TNode>, Scope> _defaultInputTransform;
		Action<InstanceConnection<TNode>, Scope> _defaultOutputTransform;


		public InstanceMatchBase(IInstanceAnalisys<TNode> owner, Func<string, object, Scope, bool> match)
		{
			_owner = owner;
			_match = match;
		}


		public IInstanceMatch<TNode> Input(Action<InstanceConnection<TNode>, Scope> transform)
		{
			if (_defaultInputTransform != null)
				throw new InvalidOperationException("duplicate default Transform");

			_defaultInputTransform = transform;
			return this;
		}

		public IInstanceMatch<TNode> Output(Action<InstanceConnection<TNode>, Scope> transform)
		{
			if (_defaultOutputTransform != null)
				throw new InvalidOperationException("duplicate default Transform");

			_defaultOutputTransform = transform;
			return this;
		}

		public IInstanceMatch<TNode> Input(InstanceConnector connector, Action<InstanceConnector, object, object, Scope> dt, Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform)
		{
			var id = connector.Id;
			if (_input.ContainsKey(id))
				throw new InvalidOperationException("duplicate input");

			if (dt != null)
				_dataTransform[connector] = dt;

			if (transform != null)
				_nodeTransform[connector] = transform;

			_input[id] = connector;
			return this;
		}

		public IInstanceMatch<TNode> Output(InstanceConnector connector, Action<InstanceConnector, object, object, Scope> dt, Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform)
		{
			var id = connector.Id;
			if (_output.ContainsKey(id))
				throw new InvalidOperationException("duplicate input");

			if (dt != null)
				_dataTransform[connector] = dt;

			if (transform != null)
				_nodeTransform[connector] = transform;

			_output[id] = connector;
			return this;
		}

		IInstanceTransform<TNode> _transform;
		public IInstanceAnalisys<TNode> Then(Func<string, object, TNode, Scope, TNode> handler)
		{
			return Then((id, instance, node, connections, scope) => handler(id, instance, node, scope));
		}

		public IInstanceAnalisys<TNode> Then(Func<string, object, Scope, TNode> handler)
		{
			return Then((id, instance, node, connections, scope) => handler(id, instance, scope));
		}

		public IInstanceAnalisys<TNode> Then(Func<string, object, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler)
		{
			return Then((id, instance, node, connections, scope) => handler(id, instance, connections, scope));
		}

		public IInstanceAnalisys<TNode> Then(Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler)
		{
			return Then(new FunctorInstanceTransform<TNode>(_input, _output, _dataTransform, _nodeTransform, _defaultInputTransform, _defaultOutputTransform, handler));
		}

		public IInstanceAnalisys<TNode> Then(IInstanceTransform<TNode> transform)
		{
			Debug.Assert(_transform == null);
			_transform = transform;
			return _owner;
		}

		internal void Apply(IInstanceDocument<TNode> document)
		{
			Debug.Assert(_match != null);
			Debug.Assert(_transform != null);
			document.Change(_match, _transform);
		}
	}
}