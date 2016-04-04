using System.Collections.Generic;
using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Excess.Extensions.Sql.Grammar;
using Microsoft.CodeAnalysis;

namespace Excess.Extensions.Sql
{
	public class TSQLGrammar : IGrammar<SyntaxToken, SyntaxNode, ParserRuleContext>
	{
		public ParserRuleContext parse(IEnumerable<SyntaxToken> tokens, Scope scope, int offset)
		{
			var text = RoslynCompiler.TokensToString(tokens);
			var stream = new AntlrInputStream(text);
			ITokenSource lexer = new tsqlLexer(stream);
			ITokenStream tokenStream = new CommonTokenStream(lexer);
			var parser = new tsqlParser(tokenStream);
			return parser.select_statement();
		}
	}
}