using System.Collections.Generic;

namespace Excess.Compiler
{
	public interface ILexicalMatchResult<TToken, TNode, TModel>
	{
		int Consumed { get; }
		IEnumerable<LexicalMatchItem> Items { get; }
		ILexicalTransform<TToken, TNode, TModel> Transform { get; set; }
		IEnumerable<TToken> GetTokens(IEnumerable<TToken> tokens, TokenSpan span);
	}
}