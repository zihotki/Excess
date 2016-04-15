using Excess.Compiler;
using Excess.Compiler.Core;
using Excess.Compiler.Roslyn;
using Excess.Entensions.XS;
using Microsoft.CodeAnalysis;

namespace Excess
{
    using Injector = ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using CompositeInjector = CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using DelegateInjector = DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>;

    public class TranslationService : ITranslationService
    {
        private RoslynCompiler _compiler;

        public string translate(string text)
        {
            if (_compiler == null)
                initCompiler();

            string rText;
            var tree = _compiler.ApplySemanticalPass(text, out rText);
            return tree.GetRoot().NormalizeWhitespace().ToString();
        }

        private void initCompiler()
        {
            _compiler = new RoslynCompiler();
            Injector injector = new CompositeInjector(new[] {XSLang.Create(), demoExtensions()});
            injector.Apply(_compiler);
        }

        private Injector demoExtensions()
        {
            return new DelegateInjector(compiler =>
            {
                Asynch.Apply(compiler);
                Match.Apply(compiler);
                Contract.Apply(compiler);
            });
        }
    }
}