using System.Collections.Generic;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Roslyn
{
    public class RoslynSyntacticalMatch : BaseSyntacticalMatch<SyntaxToken, SyntaxNode, SemanticModel>
    {
        public RoslynSyntacticalMatch(ISyntaxAnalysis<SyntaxToken, SyntaxNode, SemanticModel> syntax) :
            base(syntax)
        {
        }

        protected override IEnumerable<SyntaxNode> Children(SyntaxNode node)
        {
            return node.ChildNodes();
        }

        protected override IEnumerable<SyntaxNode> Descendants(SyntaxNode node)
        {
            return node.DescendantNodes();
        }
    }
}