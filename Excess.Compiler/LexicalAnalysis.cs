using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Excess.Compiler
{
	public interface ILexicalAnalysis<TToken, TNode, TModel>
	{
		ILexicalMatch<TToken, TNode, TModel> Match(); 
		ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<IEnumerable<TToken>, Scope, LexicalExtension<TToken>, IEnumerable<TToken>> handler);
		ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, Scope, LexicalExtension<TToken>, TNode> handler);
		IGrammarAnalysis<TGrammar, TGNode, TToken, TNode> Grammar<TGrammar, TGNode>(string keyword, ExtensionKind kind) where TGrammar : IGrammar<TToken, TNode, TGNode>, new();

		INormalizer<TToken, TNode, TModel> Normalize();

		ILexicalTransform<TToken, TNode, TModel> Transform();
	}
}
