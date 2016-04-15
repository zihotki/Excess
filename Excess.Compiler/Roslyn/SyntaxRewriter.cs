using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Excess.Compiler.Roslyn
{
    public class SyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly Scope _scope;
        private readonly IEnumerable<Func<SyntaxNode, Scope, SyntaxNode>> _transformers;

        public SyntaxRewriter(IEnumerable<Func<SyntaxNode, Scope, SyntaxNode>> transformers,
            Scope scope)
        {
            _transformers = transformers;
            _scope = scope;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node == null)
                return null;

            var result = node;
            foreach (var transformer in _transformers)
            {
                var before = result;
                result = transformer(result, _scope);

                if (result != before)
                    result = RoslynCompiler.UpdateExcessId(result, before);

                if (result == null)
                    return null;
            }

            return base.Visit(result);
        }
    }
}