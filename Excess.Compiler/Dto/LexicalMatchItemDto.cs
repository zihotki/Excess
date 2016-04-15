namespace Excess.Compiler
{
    public class LexicalMatchItemDto
    {
        public TokenSpanDto Span { get; set; }
        public string Identifier { get; set; }
        public bool Literal { get; set; }

        public LexicalMatchItemDto(TokenSpanDto span, string identifier, bool literal = false)
        {
            Span = span;
            Identifier = identifier;
            Literal = false;
        }
    }
}