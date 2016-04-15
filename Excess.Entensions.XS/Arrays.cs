using Excess.Compiler;
using Excess.Compiler.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Entensions.XS
{
    using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;

    class Arrays
    {
        static public void Apply(ExcessCompiler compiler)
        {
            compiler.Lexical()
                .Match()
                    .Any(new[] { '(', '=', ',' }, named: "start", matchDocumentStart: true)
                    .Enclosed('[', ']', start: "open", end: "close")
                    .Then(compiler.Lexical().Transform()
                        .Insert("new []", after: "start")
                        .Replace("open", "{")
                        .Replace("close", "}"));
        }
    }
}