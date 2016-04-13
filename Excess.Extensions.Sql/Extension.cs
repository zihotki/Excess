using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Excess.Extensions.Sql.Grammar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System;
using ExcessCompiler = Excess.Compiler.ICompiler<Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxNode, Microsoft.CodeAnalysis.SemanticModel>;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace Excess.Extensions.Sql
{
    public static class Extension
    {
        public static void Apply(ExcessCompiler compiler)
        {
            compiler.Lexical()
                .grammar<TSQLGrammar, ParserRuleContext>("Sql", ExtensionKind.Code)
                .transform<tsqlParser.Select_statementContext>(SelectStatement) 
				.transform<tsqlParser.Select_list_elemContext>()

                .then(Transform);
        }

        private static SyntaxNode SelectStatement(tsqlParser.Select_statementContext statement,
            Func<ParserRuleContext, Scope, SyntaxNode> transform, Scope scope)
        {
            return CSharp.Block(CSharp.EmptyStatement());
        }

        private static SyntaxNode Transform(SyntaxNode oldNode, SyntaxNode newNode, Scope scope, LexicalExtension<SyntaxToken> extension)
        {
            Debug.Assert(newNode is BlockSyntax);

            var isAssignment = oldNode is LocalDeclarationStatementSyntax;
            if (!isAssignment && oldNode is BinaryExpressionSyntax)
            {
                var expr = oldNode as BinaryExpressionSyntax;
                isAssignment = expr.Kind() == SyntaxKind.SimpleAssignmentExpression;
            }

            if (isAssignment)
            {
                scope.AddError("etsql01", "Sql always returns value", oldNode);
                return newNode;
            }

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
            document.change(oldNode.Parent, RoslynCompiler.ExplodeBlock(newNode));

            return newNode;
        }
    }
}
