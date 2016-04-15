namespace Excess.Compiler
{
    public class LexicalMatchItem
    {
        public TokenSpan Span { get; set; }
        public string Identifier { get; set; }
        public bool Literal { get; set; }

        public LexicalMatchItem(TokenSpan span, string identifier, bool literal = false)
        {
            Span = span;
            Identifier = identifier;
            Literal = false;
        }
    }
}