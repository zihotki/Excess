using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public abstract class BaseSyntacticalMatch<TToken, TNode, TModel> : ISyntacticalMatch<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        private readonly List<Func<TNode, Scope, bool>> _matchers = new List<Func<TNode, Scope, bool>>();
        private readonly ISyntaxAnalysis<TToken, TNode, TModel> _syntax;
        private Func<TNode, TNode, TModel, Scope, TNode> _semantical;

        private Func<TNode, Scope, TNode> _syntactical;

        public BaseSyntacticalMatch(ISyntaxAnalysis<TToken, TNode, TModel> syntax)
        {
            _syntax = syntax;
        }

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            document.Change(Transform);
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Children(Func<TNode, bool> selector, string named)
        {
            _matchers.Add(MatchChildren(selector, named));
            return this;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Children<T>(Func<T, bool> selector, string named) where T : TNode
        {
            _matchers.Add(MatchChildren(node => node is T && selector((T) node), named));
            return this;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Descendants(Func<TNode, bool> selector, string named)
        {
            _matchers.Add(MatchDescendants(selector, named));
            return this;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Descendants<T>(Func<T, bool> selector, string named) where T : TNode
        {
            if (selector != null)
            {
                _matchers.Add(MatchDescendants(node => node is T && selector((T) node), named));
            }
            else
            {
                _matchers.Add(MatchDescendants(node => node is T, named));
            }

            return this;
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode> handler)
        {
            return Then((node, scope) => handler(node));
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Then(ISyntaxTransform<TNode> transform)
        {
            return Then((node, scope) => transform.Transform(node, scope));
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler)
        {
            Debug.Assert(_syntactical == null && _semantical == null);
            _syntactical = handler;
            return _syntax;
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler)
        {
            Debug.Assert(_syntactical == null && _semantical == null);
            _semantical = handler;
            return _syntax;
        }

        public void AddMatcher(Func<TNode, bool> matcher)
        {
            _matchers.Add((node, scope) => matcher(node));
        }

        public void AddMatcher(Func<TNode, Scope, bool> matcher)
        {
            _matchers.Add(matcher);
        }

        public void AddMatcher<T>(Func<T, Scope, bool> matcher) where T : TNode
        {
            _matchers.Add((node, scope) => node is T && matcher((T) node, scope));
        }

        protected abstract IEnumerable<TNode> Children(TNode node);
        protected abstract IEnumerable<TNode> Descendants(TNode node);

        private Func<TNode, Scope, bool> MatchChildren(Func<TNode, bool> selector, string named)
        {
            return (node, scope) =>
            {
                var nodes = Children(node)
                    .Where(childNode => selector(childNode));

                if (named != null)
                {
                    scope.set(named, nodes);
                }

                return true;
            };
        }

        private Func<TNode, Scope, bool> MatchDescendants(Func<TNode, bool> selector, string named)
        {
            return (node, scope) =>
            {
                var nodes = Descendants(node)
                    .Where(childNode => selector(childNode));

                if (named != null)
                {
                    scope.set(named, nodes);
                }

                return true;
            };
        }

        private TNode Transform(TNode node, Scope scope)
        {
            if (_syntactical == null)
            {
                return node;
            }

            foreach (var matcher in _matchers)
            {
                if (!matcher(node, scope))
                {
                    return node;
                }
            }

            if (_syntactical != null)
            {
                return _syntactical(node, scope);
            }

            Debug.Assert(_semantical != null);
            var document = scope.GetDocument<TToken, TNode, TModel>();
            return document.Change(node, _semantical);
        }
    }
}