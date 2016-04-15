using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
	public interface ILexicalTransform<TToken, TNode, TModel>
	{
		ILexicalTransform<TToken, TNode, TModel> Insert(string tokens, string before = null, string after = null);
		ILexicalTransform<TToken, TNode, TModel> Replace(string named, string tokens);
		ILexicalTransform<TToken, TNode, TModel> Remove(string named);

		ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode> handler, string referenceToken = null);
		ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler, string referenceToken = null);
		ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler, string referenceToken = null);

		IEnumerable<TToken> Transform(IEnumerable<TToken> tokens, ILexicalMatchResult<TToken, TNode, TModel> match, Scope scope);
	}
}