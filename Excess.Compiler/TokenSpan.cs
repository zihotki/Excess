namespace Excess.Compiler
{
	public class TokenSpan
	{
		public TokenSpan(int start, int length)
		{
			Start  = start;
			Length = length;
		}

		public int Start { get; set; }
		public int Length { get; set; }
	}
}