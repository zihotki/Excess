using System.Linq;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Entensions.XS
{
    using CSharp = SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;

    public class Asynch
    {
        private static readonly StatementSyntax ContextVariable = CSharp.ParseStatement(@"
            SynchronizationContext __ASynchCtx = SynchronizationContext.Current; ");

        private static readonly StatementSyntax AsynchTemplate = CSharp.ParseStatement(@"
            Task.Factory.StartNew(() =>
            {
            });");

        private static readonly StatementSyntax SynchTemplate = CSharp.ParseStatement(@"
            __ASynchCtx.Post(() => 
            { 
            });");

        public static void Apply(ExcessCompiler compiler)
        {
            var syntax = compiler.Syntax;

            //code extension
            syntax
                .Extension("asynch", ExtensionKind.Code, ProcessAsynch)
                .Extension("synch", ExtensionKind.Code, ProcessSynch);
        }

        private static SyntaxNode ProcessAsynch(SyntaxNode node, Scope scope, SyntacticalExtensionDto<SyntaxNode> extension)
        {
            if (extension.Kind == ExtensionKind.Code)
            {
                var result = AsynchTemplate
                    .ReplaceNodes(AsynchTemplate
                        .DescendantNodes()
                        .OfType<BlockSyntax>(),
                        (oldNode, newNode) => extension.Body);

                var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                document.Change(node.Parent, RoslynCompiler.AddStatement(ContextVariable, node));

                return result;
            }

            scope.AddError("asynch01", "asynch does not return a value", node);
            return node;
        }

        private static SyntaxNode ProcessSynch(SyntaxNode node, Scope scope, SyntacticalExtensionDto<SyntaxNode> extension)
        {
            if (extension.Kind == ExtensionKind.Code)
            {
                //TODO: verify it's inside an asynch
                return SynchTemplate
                    .ReplaceNodes(SynchTemplate
                        .DescendantNodes()
                        .OfType<BlockSyntax>(),
                        (oldNode, newNode) => extension.Body);
            }

            scope.AddError("synch01", "synch does not return a value", node);
            return node;
        }
    }
}