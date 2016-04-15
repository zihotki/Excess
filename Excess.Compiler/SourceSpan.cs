namespace Excess.Compiler
{
    public class SourceSpan
    {
        public int Start { get; internal set; }
        public int Length { get; internal set; }

        public SourceSpan()
        {
        }

        public SourceSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}