using System.Collections.Generic;
using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;

namespace Excess.Extensions.R
{
    public class RGrammar : IGrammar<SyntaxToken, SyntaxNode, ParserRuleContext>
    {
        public ParserRuleContext Parse(IEnumerable<SyntaxToken> tokens, Scope scope, int offset)
        {
            var text = RoslynCompiler.TokensToString(tokens);
            var stream = new AntlrInputStream(text);
            ITokenSource lexer = new RLexer(stream);
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            var parser = new RParser(tokenStream);
            return parser.prog();
        }
    }
}