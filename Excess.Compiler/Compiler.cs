using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excess.Compiler
{
	public interface ICompilerService<TToken, TNode, TModel>
    {
        string TokenToString(TToken token, out string xsId);
        string TokenToString(TToken token, out int xsId);
        string TokenToString(TToken token);
        TToken MarkToken(TToken token, out int xsId);
        TToken MarkToken(TToken token);
        TNode MarkNode(TNode node, out int xsId);
        TNode MarkNode(TNode node);
        TToken InitToken(TToken token, int xsId);
        TNode MarkTree(TNode node);
        int GetExcessId(TToken token);
        int GetExcessId(TNode node);
        bool IsIdentifier(TToken token);

        IEnumerable<TToken> ParseTokens(string text);
        TNode Parse(string text);
        IEnumerable<TToken> MarkTokens(IEnumerable<TToken> tokens, out int xsId);
        IEnumerable<TNode> Find(TNode node, IEnumerable<string> xsIds);
        TNode Find(TNode node, SourceSpan value);
        int GetOffset(TToken token);
    }
}
