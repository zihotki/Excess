using System;

namespace Excess.Compiler
{
    public class SyntacticalExtensionDto<TNode>
    {
        public ExtensionKind Kind { get; set; }
        public string Keyword { get; set; }
        public string Identifier { get; set; }
        public TNode Arguments { get; set; }
        public TNode Body { get; set; }
        public Func<TNode, Scope, SyntacticalExtensionDto<TNode>, TNode> Handler { get; set; }

        public SyntacticalExtensionDto()
        {
        }

        public SyntacticalExtensionDto(string keyword, ExtensionKind kind, Func<TNode, Scope, SyntacticalExtensionDto<TNode>, TNode> handler)
        {
            Keyword = keyword;
            Kind = kind;
            Handler = handler;
        }
    }
}