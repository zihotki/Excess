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
            {
                return null;
            }

            foreach (var transformer in _transformers)
            {
                var before = node;
                node = transformer(node, _scope);

                if (node != before)
                {
                    node = RoslynCompiler.UpdateExcessId(node, before);
                }

                if (node == null)
                {
                    return null;
                }
            }

            return base.Visit(node);
        }
    }
}