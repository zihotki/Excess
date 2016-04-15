using System;
using System.Collections.Generic;
using Excess.Compiler.Core;
using Microsoft.CodeAnalysis;

namespace Excess.Compiler.Roslyn
{
    public class RoslynSyntaxTransform : BaseSyntaxTransform<SyntaxToken, SyntaxNode, SemanticModel>
    {
        public RoslynSyntaxTransform()
        {
        }

        public RoslynSyntaxTransform(Func<SyntaxNode, Scope, IEnumerable<SyntaxNode>, SyntaxNode> handler)
        {
            Selectors.Add(null);
            Transformers.Add(handler);
        }

        protected override SyntaxNode AddToNode(SyntaxNode node, IEnumerable<SyntaxNode> nodes)
        {
            throw new NotImplementedException(); //td: 
        }

        protected override SyntaxNode RemoveNodes(SyntaxNode node, IEnumerable<SyntaxNode> nodes)
        {
            return node.RemoveNodes(nodes, SyntaxRemoveOptions.KeepEndOfLine);
        }

        protected override SyntaxNode ReplaceNodes(SyntaxNode node, Scope scope, IEnumerable<SyntaxNode> nodes, Func<SyntaxNode, Scope, SyntaxNode> handler)
        {
            return node.ReplaceNodes(nodes, (oldNode, newNode) =>
            {
                //change the result temporarily, this might need revisiting
                var returnValue = handler(newNode, scope);
                return returnValue;
            });
        }

        protected override SyntaxNode ResolveScope(SyntaxNode node, bool type, bool @namespace)
        {
            throw new NotImplementedException();
        }
    }
}