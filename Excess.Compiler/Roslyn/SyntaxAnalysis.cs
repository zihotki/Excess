using System;
using System.Collections.Generic;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Excess.Compiler.Roslyn
{
    using CSharp = SyntaxFactory;

    public class RoslynSyntaxAnalysis : BaseSyntaxAnalysis<SyntaxToken, SyntaxNode, SemanticModel>
    {
        protected override ISyntaxTransform<SyntaxNode> CreateTransform()
        {
            return new RoslynSyntaxTransform();
        }

        protected override ISyntacticalMatch<SyntaxToken, SyntaxNode, SemanticModel> CreateMatch(Func<SyntaxNode, bool> selector)
        {
            var result = new RoslynSyntacticalMatch(this);
            result.AddMatcher(selector);
            return result;
        }

        protected override ISyntaxTransform<SyntaxNode> CreateTransform(Func<SyntaxNode, Scope, IEnumerable<SyntaxNode>, SyntaxNode> handler)
        {
            return new RoslynSyntaxTransform(handler);
        }

        protected override SyntaxNode Extensions(SyntaxNode node, Scope scope)
        {
            var rewriter = new ExtensionRewriter(_extensions, scope);
            return rewriter.Visit(node);
        }
    }
}