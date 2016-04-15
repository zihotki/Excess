using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
	public interface IInstanceTransform<TNode>
	{
		TNode Transform(string id, object instance, TNode node, IEnumerable<InstanceConnection<TNode>> connections, Scope scope);
		InstanceConnector Output(
			string connector,
			out Action<InstanceConnector, object, object, Scope> dt,
			out Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform);
		InstanceConnector Input(
			string connector,
			out Action<InstanceConnector, object, object, Scope> dt,
			out Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform);
	}
}