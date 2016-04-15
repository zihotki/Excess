using System.Collections.Generic;

namespace Excess.Compiler.Core
{
	public class Instance<TNode>
	{
		public string Id { get; set; }
		public string Class { get; set; }
		public object Value { get; set; }
		public TNode Node { get; set; }

		internal IInstanceTransform<TNode> Transform { get; set; }
		internal List<InstanceConnection<TNode>> Connections { get; set; }

		public Instance()
		{
			Connections = new List<InstanceConnection<TNode>>();
		}
	}
}