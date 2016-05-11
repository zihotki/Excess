using Antlr4.Runtime;

namespace Excess.Compiler.Roslyn
{
    public class AntlrErrors<TToken> : IAntlrErrorListener<TToken>
        where TToken : IToken
    {
        private readonly int _offset;
        private readonly Scope _scope;

        public AntlrErrors(Scope scope, int offset)
        {
            _scope = scope;
            _offset = offset;
        }

        public void SyntaxError(IRecognizer recognizer, TToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            _scope.AddError("grammar syntax error", msg, _offset + offendingSymbol.StartIndex,
                offendingSymbol.StopIndex - offendingSymbol.StartIndex);
        }
    }
}