using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
	public abstract class BaseSyntaxAnalysis<TToken, TNode, TModel> : ISyntaxAnalysis<TToken, TNode, TModel>,
		IDocumentInjector<TToken, TNode, TModel>
	{
		protected List<SyntacticalExtension<TNode>> _extensions = new List<SyntacticalExtension<TNode>>();

		protected List<ISyntacticalMatch<TToken, TNode, TModel>> _matchers = new List<ISyntacticalMatch<TToken, TNode, TModel>>();

		public void Apply(IDocument<TToken, TNode, TModel> document)
		{
			if (_extensions.Any())
			{
				document.Change(Extensions, "syntactical-extensions");
			}

			foreach (var matcher in _matchers)
			{
				var handler = matcher as IDocumentInjector<TToken, TNode, TModel>;
				Debug.Assert(handler != null);

				handler.Apply(document);
			}
		}

		public ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, Scope, SyntacticalExtension<TNode>, TNode> handler)
		{
			_extensions.Add(new SyntacticalExtension<TNode>(keyword, kind, handler));
			return this;
		}

		public ISyntaxAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, SyntacticalExtension<TNode>, TNode> handler)
		{
			_extensions.Add(new SyntacticalExtension<TNode>(keyword, kind, (node, scope, ext) => handler(node, ext)));
			return this;
		}

		public ISyntacticalMatch<TToken, TNode, TModel> Match(Func<TNode, bool> selector)
		{
			var matcher = CreateMatch(selector);
			_matchers.Add(matcher);

			return matcher;
		}

		public ISyntacticalMatch<TToken, TNode, TModel> Match<T>(Func<T, bool> selector) where T : TNode
		{
			var matcher = CreateMatch(node => node is T && selector((T) node));
			_matchers.Add(matcher);

			return matcher;
		}

		public ISyntacticalMatch<TToken, TNode, TModel> Match<T>() where T : TNode
		{
			var matcher = CreateMatch(node => node is T);
			_matchers.Add(matcher);

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

		protected abstract TNode Extensions(TNode node, Scope scope);
	}
}