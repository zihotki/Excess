﻿using Antlr4.Runtime;
using Excess.Compiler;
using ExcessCompiler = Excess.Compiler.ICompiler<Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxNode, Microsoft.CodeAnalysis.SemanticModel>;


namespace Excess.Extensions.Sql
{
	public class Extension
	{
		public static void Apply(ExcessCompiler compiler)
		{
			compiler.Lexical()
				.grammar<TSQLGrammar, ParserRuleContext>("Sql", ExtensionKind.Code);
		}
	}
}