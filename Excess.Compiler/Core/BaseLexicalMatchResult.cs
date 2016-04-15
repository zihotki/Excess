using System.Collections.Generic;
using System.Linq;

namespace Excess.Compiler.Core
{
    public class BaseLexicalMatchResult<TToken, TNode, TModel> : ILexicalMatchResult<TToken, TNode, TModel>
    {
        public BaseLexicalMatchResult(IEnumerable<LexicalMatchItemDto> items, ILexicalTransform<TToken, TNode, TModel> transform)
        {
            Items = items;
            Transform = transform;

            Consumed = 0;
            foreach (var item in items)
            {
                Consumed += item.Span.Length;
            }
        }

        public int Consumed { get; }
        public IEnumerable<LexicalMatchItemDto> Items { get; }
        public ILexicalTransform<TToken, TNode, TModel> Transform { get; set; }

        public IEnumerable<TToken> GetTokens(IEnumerable<TToken> tokens, TokenSpanDto span)
        {
            return tokens
                .Skip(span.Start)
                .Take(span.Length);
        }
    }
}