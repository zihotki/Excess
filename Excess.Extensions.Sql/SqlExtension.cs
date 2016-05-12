using System;
using System.Diagnostics;
using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Excess.Extensions.Sql.Grammar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using ExcessCompiler = Excess.Compiler.ICompiler<Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxNode, Microsoft.CodeAnalysis.SemanticModel>;


namespace Excess.Extensions.Sql
{
	public static class SqlExtension
	{
		

		public static void Apply(ExcessCompiler compiler)
		{
			compiler.Lexical
				.Grammar<TSQLGrammar, ParserRuleContext>("Sql", ExtensionKind.Code)
				.Transform<tsqlParser.Select_statementContext>(SelectStatement)
				.Transform<tsqlParser.Select_list_elemContext>(SelectListElementStatement)
				.Transform<tsqlParser.Table_sourceContext>(TableSourceContext)
				.Then(Transform);
		}

		private static SyntaxNode TableSourceContext(tsqlParser.Table_sourceContext arg1, Func<ParserRuleContext, Scope, SyntaxNode> arg2, Scope arg3)
		{
			throw new NotImplementedException();
		}

		private static SyntaxNode SelectListElementStatement(tsqlParser.Select_list_elemContext arg1, Func<ParserRuleContext, Scope, SyntaxNode> arg2,
			Scope arg3)
		{
			throw new NotImplementedException();
		}

		private static SyntaxNode SelectStatement(tsqlParser.Select_statementContext statement,
			Func<ParserRuleContext, Scope, SyntaxNode> transform, Scope scope)
		{
			return CSharp.EqualsValueClause(CSharp.ObjectCreationExpression(CSharp.IdentifierName("Select")));


/*				Block(
				ExpressionStatement(
					InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("C"), IdentifierName("Hello")))
						.WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("aa")))))))
						
						);*/
		}

		private static SyntaxNode Transform(SyntaxNode oldNode, SyntaxNode newNode, Scope scope, LexicalExtensionDto<SyntaxToken> extension)
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
				scope.AddError("sqldsl01", "Sql always returns value", oldNode);
				return newNode;
			}

			var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
			document.Change(oldNode.Parent, RoslynCompiler.ExplodeBlock(newNode));

			return newNode;
		}
	}
}