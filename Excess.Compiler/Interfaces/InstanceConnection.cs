using System;

namespace Excess.Compiler
{
    public class InstanceConnection<TNode>
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public InstanceConnector Output { get; set; }
        public InstanceConnector Input { get; set; }

        public object OutputModel { get; set; }
        public TNode OutputNode { get; set; }
        public TNode OutputModelNode { get; set; }
        public object InputModel { get; set; }
        public TNode InputNode { get; set; }
        public TNode InputModelNode { get; set; }

        public Action<InstanceConnector, object, object, Scope> InputDt { get; set; }
        public Action<InstanceConnector, object, object, Scope> OutputDt { get; set; }
        public Action<InstanceConnector, InstanceConnection<TNode>, Scope> InputTransform { get; set; }
        public Action<InstanceConnector, InstanceConnection<TNode>, Scope> OutputTransform { get; set; }
    }
}