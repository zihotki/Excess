using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public abstract class BaseSyntaxAnalysis<TToken, TNode, TModel> : ISyntaxAnalysis<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        protected List<SyntacticalExtensionDto<TNode>> Extensions = new List<SyntacticalExtensionDto<TNode>>();

        protected List<ISyntacticalMatch<TToken, TNode, TModel>> Matchers = new List<ISyntacticalMatch<TToken, TNode, TModel>>();

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            if (Extensions.Any())
            {
                document.Change(Rewrite, "syntactical-extensions");
            }

            foreach (var matcher in Matchers)
            {
                var handler = matcher as IDocumentInjector<TToken, TNode, TModel>;
                Debug.Assert(handler != null);

                handler.Apply(document);
            }
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind,
            Func<TNode, Scope, SyntacticalExtensionDto<TNode>, TNode> handler)
        {
            Extensions.Add(new SyntacticalExtensionDto<TNode>(keyword, kind, handler));
            return this;
        }

        public ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, SyntacticalExtensionDto<TNode>, TNode> handler)
        {
            Extensions.Add(new SyntacticalExtensionDto<TNode>(keyword, kind, (node, scope, ext) => handler(node, ext)));
            return this;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Match(Func<TNode, bool> selector)
        {
            var matcher = CreateMatch(selector);
            Matchers.Add(matcher);

            return matcher;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Match<T>(Func<T, bool> selector) where T : TNode
        {
            var matcher = CreateMatch(node => node is T && selector((T) node));
            Matchers.Add(matcher);

            return matcher;
        }

        public ISyntacticalMatch<TToken, TNode, TModel> Match<T>() where T : TNode
        {
            var matcher = CreateMatch(node => node is T);
            Matchers.Add(matcher);

            return matcher;
        }

        public ISyntaxTransform<TNode> Transform()
        {
            return CreateTransform();
        }

        public ISyntaxTransform<TNode> Transform(Func<TNode, TNode> handler)
        {
            return Transform((node, scope) => handler(node));
        }

        public ISyntaxTransform<TNode> Transform(Func<TNode, Scope, TNode> handler)
        {
            return CreateTransform((node, scope, childre) => handler(node, scope));
        }

        protected abstract ISyntacticalMatch<TToken, TNode, TModel> CreateMatch(Func<TNode, bool> selector);

        protected abstract ISyntaxTransform<TNode> CreateTransform();
        protected abstract ISyntaxTransform<TNode> CreateTransform(Func<TNode, Scope, IEnumerable<TNode>, TNode> handler);

        protected abstract TNode Rewrite(TNode node, Scope scope);
    }
}