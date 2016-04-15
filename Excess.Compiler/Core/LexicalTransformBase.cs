using System;
using System.Collections.Generic;
using System.Linq;

namespace Excess.Compiler.Core
{
	public class LexicalFunctorTransform<TToken, TNode, TModel> : ILexicalTransform<TToken, TNode, TModel>
	{
		private readonly Func<IEnumerable<TToken>, ILexicalMatchResult<TToken, TNode, TModel>, Scope, IEnumerable<TToken>> _functor;

		public LexicalFunctorTransform(Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> functor)
		{
			_functor = WithScope(functor);
		}

		public LexicalFunctorTransform(Func<IEnumerable<TToken>, ILexicalMatchResult<TToken, TNode, TModel>, Scope, IEnumerable<TToken>> functor)
		{
			_functor = functor;
		}

		public ILexicalTransform<TToken, TNode, TModel> Insert(string tokens, string before = null, string after = null)
		{
			throw new InvalidOperationException();
		}

		public ILexicalTransform<TToken, TNode, TModel> Replace(string named, string tokens)
		{
			throw new InvalidOperationException();
		}

		public ILexicalTransform<TToken, TNode, TModel> Remove(string named)
		{
			throw new InvalidOperationException();
		}

		public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode> handler, string token)
		{
			throw new InvalidOperationException();
		}

		public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler, string token)
		{
			throw new InvalidOperationException();
		}

		public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler, string token)
		{
			throw new InvalidOperationException();
		}

		public IEnumerable<TToken> Transform(IEnumerable<TToken> tokens, ILexicalMatchResult<TToken, TNode, TModel> match, Scope scope)
		{
			return _functor(tokens, match, scope);
		}

		private static Func<IEnumerable<TToken>, ILexicalMatchResult<TToken, TNode, TModel>, Scope, IEnumerable<TToken>> WithScope(
			Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> functor)
		{
			return (tokens, match, scope) =>
			{
				foreach (var item in match.Items)
				{
					if (item.Identifier != null && item.Span.Length > 0)
					{
						var idTokens = match.GetTokens(tokens, item.Span);

						if (item.Span.Length == 1)
						{
							scope.set(item.Identifier, idTokens.First());
						}
						else
						{
							scope.set(item.Identifier, idTokens);
						}
					}
				}

				return functor(tokens, scope);
			};
		}

		public ILexicalTransform<TToken, TNode, TModel> then(ISyntaxTransform<TNode> transform, string token)
		{
			throw new InvalidOperationException();
		}
	}
}