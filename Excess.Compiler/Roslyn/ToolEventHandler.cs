using System.Collections.Generic;

namespace Excess.Compiler.Roslyn
{
    public class ToolEventArgs
    {
        public ICompilationTool Tool { get; set; }
        public string Document { get; set; }
        public Dictionary<string, string> Result { get; set; }
    }

    public delegate void ToolEventHandler(object sender, ToolEventArgs e);
}