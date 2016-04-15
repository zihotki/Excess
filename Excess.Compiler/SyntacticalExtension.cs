using System;

namespace Excess.Compiler
{
	public class SyntacticalExtension<TNode>
	{
		public ExtensionKind Kind { get; set; }
		public string Keyword { get; set; }
		public string Identifier { get; set; }
		public TNode Arguments { get; set; }
		public TNode Body { get; set; }
		public Func<TNode, Scope, SyntacticalExtension<TNode>, TNode> Handler { get; set; }

		public SyntacticalExtension()
		{
		}

		public SyntacticalExtension(string keyword, ExtensionKind kind, Func<TNode, Scope, SyntacticalExtension<TNode>, TNode> handler)
		{
			Keyword = keyword;
			Kind = kind;
			Handler = handler;
		}
	}
}