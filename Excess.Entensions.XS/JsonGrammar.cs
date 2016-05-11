using System.Collections.Generic;
using Antlr4.Runtime;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Excess.Extensions.XS.Grammars;
using Microsoft.CodeAnalysis;

namespace Excess.Entensions.XS
{
    internal class JsonGrammar : IGrammar<SyntaxToken, SyntaxNode, ParserRuleContext>
    {
        public ParserRuleContext Parse(IEnumerable<SyntaxToken> tokens, Scope scope, int offset)
        {
            var text = RoslynCompiler.TokensToString(tokens);
            var stream = new AntlrInputStream(text);
            ITokenSource lexer = new JSONLexer(stream);
            ITokenStream tokenStream = new CommonTokenStream(lexer);
            var parser = new JSONParser(tokenStream);

            parser.AddErrorListener(new AntlrErrors<IToken>(scope, offset));
            var result = parser.json();
            if (parser.NumberOfSyntaxErrors > 0)
                return null;

            return result;
        }
    }
}