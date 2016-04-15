using System;
using System.Collections.Generic;
using System.Linq;

namespace Excess.Compiler.Core
{
	public class BaseGrammarAnalysis<TToken, TNode, TModel, GNode, TGrammar> : IGrammarAnalysis<TGrammar, GNode, TToken, TNode>
		where TGrammar : IGrammar<TToken, TNode, GNode>, new()
	{
		private readonly TGrammar _grammar;

		private Func<TNode, TNode, Scope, LexicalExtension<TToken>, TNode> _then;

		private readonly Dictionary<Type, Func<GNode, Func<GNode, Scope, TNode>, Scope, TNode>> _transformers =
			new Dictionary<Type, Func<GNode, Func<GNode, Scope, TNode>, Scope, TNode>>();

		public BaseGrammarAnalysis(ILexicalAnalysis<TToken, TNode, TModel> lexical, string keyword, ExtensionKind kind)
		{
			lexical.Extension(keyword, kind, ParseExtension);
			_grammar = new TGrammar();
		}

		public void Then(Func<TNode, TNode, Scope, LexicalExtension<TToken>, TNode> handler)
		{
			_then = handler;
		}

		public IGrammarAnalysis<TGrammar, GNode, TToken, TNode> Transform<T>(Func<T, Func<GNode, Scope, TNode>, Scope, TNode> handler) where T : GNode
		{
			var type = typeof (T);
			if (_transformers.ContainsKey(type))
			{
				throw new InvalidOperationException("multiple type handlers");
			}

			_transformers[type] = (node, grammar, scope) => handler((T) node, DoTransform, scope);
			return this;
		}

		private TNode ParseExtension(TNode node, Scope scope, LexicalExtension<TToken> extension)
		{
			var allTokens = extension.Body.ToArray();
			var withoutBraces = Range(allTokens, 1, allTokens.Length - 1);
			if (!withoutBraces.Any())
			{
				return node;
			}

			var compiler = scope.GetService<TToken, TNode, TModel>();
			var g = _grammar.Parse(withoutBraces, scope, compiler.GetOffset(withoutBraces.First()));

			if (g == null || g.Equals(default(GNode)))
			{
				return node; //errors added to the scope already
			}

			var result = DoTransform(g, scope);
			if (_then != null)
			{
				result = _then(node, result, scope, extension);
			}

			return result;
		}

		private IEnumerable<TToken> Range(TToken[] tokens, int from, int to)
		{
			for (var i = from; i < to; i++)
			{
				yield return tokens[i];
			}
		}

		private TNode DoTransform(GNode g, Scope scope)
		{
			var type = g.GetType();
			var handler = null as Func<GNode, Func<GNode, Scope, TNode>, Scope, TNode>;
			if (_transformers.TryGetValue(type, out handler))
			{
				return handler(g, DoTransform, scope);
			}

			return default(TNode);
		}
	}
}