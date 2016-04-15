using Excess.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Excess.Entensions.XS
{
    using CSharp = SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;

    internal class Arrays
    {
        public static void Apply(ExcessCompiler compiler)
        {
            compiler.Lexical()
                .Match()
                .Any(new[] {'(', '=', ','}, "start", true)
                .Enclosed('[', ']', "open", "close")
                .Then(compiler.Lexical().Transform()
                    .Insert("new []", after: "start")
                    .Replace("open", "{")
                    .Replace("close", "}"));
        }
    }
}