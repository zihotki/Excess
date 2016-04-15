using Excess.Compiler;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Excess.Entensions.XS
{
    using CSharp = SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;
    using Injector = ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using DelegateInjector = DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>;

    public class XSLang
    {
        public static void Apply(ExcessCompiler compiler)
        {
            Functions.Apply(compiler);
            Members.Apply(compiler);
            Events.Apply(compiler);
            TypeDef.Apply(compiler);
            Arrays.Apply(compiler);
        }

        public static Injector Create()
        {
            return new DelegateInjector(compiler => Apply(compiler));
        }
    }
}