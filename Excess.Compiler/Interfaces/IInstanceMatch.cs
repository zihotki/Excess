using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface IInstanceMatch<TNode>
    {
        IInstanceMatch<TNode> Input(
            InstanceConnector connector,
            Action<InstanceConnector, object, object, Scope> dt = null,
            Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform = null);

        IInstanceMatch<TNode> Output(
            InstanceConnector connector,
            Action<InstanceConnector, object, object, Scope> dt = null,
            Action<InstanceConnector, InstanceConnection<TNode>, Scope> transform = null);

        IInstanceMatch<TNode> Input(Action<InstanceConnection<TNode>, Scope> transform);
        IInstanceMatch<TNode> Output(Action<InstanceConnection<TNode>, Scope> transform);

        IInstanceAnalisys<TNode> Then(Func<string, object, TNode, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler);
        IInstanceAnalisys<TNode> Then(Func<string, object, IEnumerable<InstanceConnection<TNode>>, Scope, TNode> handler);
        IInstanceAnalisys<TNode> Then(Func<string, object, TNode, Scope, TNode> handler);
        IInstanceAnalisys<TNode> Then(Func<string, object, Scope, TNode> handler);
        IInstanceAnalisys<TNode> Then(IInstanceTransform<TNode> transform);
    }
}