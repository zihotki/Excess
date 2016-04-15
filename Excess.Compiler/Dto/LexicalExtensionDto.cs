using System.Collections.Generic;

namespace Excess.Compiler
{
    public class LexicalExtensionDto<TToken>
    {
        public ExtensionKind Kind { get; set; }
        public TToken Keyword { get; set; }
        public TToken Identifier { get; set; }
        public IEnumerable<TToken> Arguments { get; set; }
        public IEnumerable<TToken> Body { get; set; }
        public int BodyStart { get; set; }
    }
}