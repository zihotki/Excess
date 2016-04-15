using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
	public interface INormalizer<TToken, TNode, TModel>
	{
		ILexicalAnalysis<TToken, TNode, TModel> With(Func<TNode, IEnumerable<TNode>, Scope, TNode> statements = null,
			Func<TNode, IEnumerable<TNode>, Scope, TNode> members = null,
			Func<TNode, IEnumerable<TNode>, Scope, TNode> types = null,
			Func<TNode, Scope, TNode> then = null);

		ILexicalAnalysis<TToken, TNode, TModel> Statements(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler);
		ILexicalAnalysis<TToken, TNode, TModel> Members(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler);
		ILexicalAnalysis<TToken, TNode, TModel> Types(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler);
		ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler);
	}
}