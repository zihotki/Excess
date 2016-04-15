namespace Excess.Compiler
{
    public class TokenSpanDto
    {
        public int Start { get; set; }
        public int Length { get; set; }

        public TokenSpanDto(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}