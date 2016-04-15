namespace Excess.Compiler
{
    public class TokenSpan
    {
        public int Start { get; set; }
        public int Length { get; set; }

        public TokenSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}