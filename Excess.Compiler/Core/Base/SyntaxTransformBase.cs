using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public abstract class BaseSyntaxTransform<TToken, TNode, TModel> : ISyntaxTransform<TNode>
    {
        protected List<Func<TNode, Scope, IEnumerable<TNode>>> Selectors = new List<Func<TNode, Scope, IEnumerable<TNode>>>();
        protected List<Func<TNode, Scope, IEnumerable<TNode>, TNode>> Transformers = new List<Func<TNode, Scope, IEnumerable<TNode>, TNode>>();

        public ISyntaxTransform<TNode> AddToScope(Func<TNode, Scope, IEnumerable<TNode>> handler, bool type = false, bool @namespace = false)
        {
            Selectors.Add(handler);
            Transformers.Add(AddToScope(type, @namespace));
            return this;
        }

        public ISyntaxTransform<TNode> AddToScope(string nodes, bool type = false, bool @namespace = false)
        {
            Selectors.Add(SelectFromScope(nodes));
            Transformers.Add(AddToScope(type, @namespace));
            return this;
        }

        public ISyntaxTransform<TNode> Remove(string nodes)
        {
            Selectors.Add(SelectFromScope(nodes));
            Transformers.Add(RemoveNodes());
            return this;
        }

        public ISyntaxTransform<TNode> Remove(Func<TNode, Scope, IEnumerable<TNode>> selector)
        {
            Selectors.Add(selector);
            Transformers.Add(RemoveNodes());
            return this;
        }

        public ISyntaxTransform<TNode> Replace(string nodes, Func<TNode, TNode> handler)
        {
            Selectors.Add(SelectFromScope(nodes));
            Transformers.Add(ReplaceNodes((node, scope) => handler(node)));
            return this;
        }

        public ISyntaxTransform<TNode> Replace(string nodes, Func<TNode, Scope, TNode> handler)
        {
            Selectors.Add(SelectFromScope(nodes));
            Transformers.Add(ReplaceNodes(handler));
            return this;
        }

        public ISyntaxTransform<TNode> Replace(Func<TNode, Scope, IEnumerable<TNode>> selector, Func<TNode, Scope, TNode> handler)
        {
            Selectors.Add(selector);
            Transformers.Add(ReplaceNodes(handler));
            return this;
        }

        public ISyntaxTransform<TNode> Replace(Func<TNode, Scope, IEnumerable<TNode>> selector, Func<TNode, TNode> handler)
        {
            Selectors.Add(selector);
            Transformers.Add(ReplaceNodes((node, scope) => handler(node)));
            return this;
        }

        public TNode Transform(TNode node, Scope scope)
        {
            var compiler = scope.GetService<TToken, TNode, TModel>();

            Debug.Assert(compiler != null && Selectors.Count == Transformers.Count);
            switch (Transformers.Count)
            {
                case 0:
                    return node;
                case 1:
                {
                    //do not track on single transformations
                    var selector = Selectors[0];
                    var nodes = selector != null
                        ? selector(node, scope)
                        : new TNode[] {};
                    var resultNode = Transformers[0](node, scope, nodes);
                    return resultNode;
                }
                default:
                {
                    //problem here is there are dependency nodes obtained 
                    //during match, so tracking should be performed.
                    //I never got the roslyn one to work for some reason.
                    //so, td:

                    var selectorIds = new Dictionary<object, List<string>>();
                    foreach (var selector in Selectors)
                    {
                        var sNodes = selector(node, scope);
                        if (sNodes.Any())
                        {
                            foreach (var sNode in sNodes)
                            {
                                var xsid = compiler.GetExcessId(sNode).ToString();

                                List<string> selectorNodes;
                                if (!selectorIds.TryGetValue(selector, out selectorNodes))
                                {
                                    selectorNodes = new List<string>();
                                    selectorIds[selector] = selectorNodes;
                                }

                                selectorNodes.Add(xsid);
                            }
                        }
                    }

                    for (var i = 0; i < Transformers.Count; i++)
                    {
                        var transformer = Transformers[i];
                        var selector = Selectors[i];

                        IEnumerable<TNode> nodes = null;
                        List<string> nodeIds;
                        if (selectorIds.TryGetValue(selector, out nodeIds))
                        {
                            nodes = compiler.Find(node, nodeIds);
                        }

                        node = transformer(node, scope, nodes);
                        if (node == null)
                        {
                            return default(TNode);
                        }
                    }

                    return node;
                }
            }
        }

        private Func<TNode, Scope, IEnumerable<TNode>, TNode> AddToScope(bool type, bool @namespace)
        {
            return (node, scope, nodes) =>
            {
                if (nodes == null)
                {
                    return node;
                }

                var scopeNode = ResolveScope(node, type, @namespace);
                if (scopeNode != null)
                {
                    var document = scope.GetDocument<TToken, TNode, TModel>();
                    return document.Change(scopeNode, (n, s) => AddToNode(n, nodes));
                }

                return node;
            };
        }

        protected abstract TNode ResolveScope(TNode node, bool type, bool @namespace);

        private Func<TNode, Scope, IEnumerable<TNode>> SelectFromScope(string nodes)
        {
            return (node, scope) => scope.Get<IEnumerable<TNode>>(nodes);
        }

        private Func<TNode, Scope, IEnumerable<TNode>, TNode> RemoveNodes()
        {
            return (node, scope, nodes) =>
            {
                if (nodes == null)
                {
                    return node;
                }

                return RemoveNodes(node, nodes);
            };
        }

        private Func<TNode, Scope, IEnumerable<TNode>, TNode> ReplaceNodes(Func<TNode, Scope, TNode> handler)
        {
            return (node, scope, nodes) =>
            {
                if (nodes == null || !nodes.Any())
                {
                    return node;
                }

                return ReplaceNodes(node, scope, nodes, handler);
            };
        }

        protected abstract TNode RemoveNodes(TNode node, IEnumerable<TNode> nodes);
        protected abstract TNode ReplaceNodes(TNode node, Scope scope, IEnumerable<TNode> nodes, Func<TNode, Scope, TNode> handler);
        protected abstract TNode AddToNode(TNode node, IEnumerable<TNode> nodes);
    }
}