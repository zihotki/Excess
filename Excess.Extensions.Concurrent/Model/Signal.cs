using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Extensions.Concurrent.Model
{
    using CSharp = SyntaxFactory;
    using Roslyn = RoslynCompiler;

    internal class Signal
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public bool Internal { get; internal set; }
        public bool Public { get; internal set; }
        public TypeSyntax ReturnType { get; internal set; }

        public Signal(int id, string name, bool @public)
        {
            Id = id;
            Name = name;
            Internal = false;
            ReturnType = RoslynCompiler.boolean;
            Public = @public;
        }
    }
}