using System.Collections.Generic;

namespace Excess.Compiler.Roslyn
{
	public interface ICompilationTool
	{
		string DisplayName { get; }
		bool DoNotCache { get; }
		bool Compile(string file, string contents, Scope scope, Dictionary<string, string> result);
	}
}