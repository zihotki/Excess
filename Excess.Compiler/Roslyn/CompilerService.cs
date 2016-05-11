using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Excess.Compiler.Roslyn
{
    public class CompilerService : ICompilerService<SyntaxToken, SyntaxNode, SemanticModel>
    {
        public string TokenToString(SyntaxToken token, out string xsId)
        {
            xsId = RoslynCompiler.TokenMark(token);
            return token.ToFullString();
        }

        public string TokenToString(SyntaxToken token, out int xsId)
        {
            var xsStr = RoslynCompiler.TokenMark(token);
            xsId = xsStr == null
                ? -1
                : int.Parse(xsStr);
            return token.ToFullString();
        }

        public string TokenToString(SyntaxToken token)
        {
            return token.ToFullString();
        }

        public SyntaxToken MarkToken(SyntaxToken token, out int xsId)
        {
            var strId = RoslynCompiler.UniqueId();
            xsId = int.Parse(strId);
            return RoslynCompiler.MarkToken(token, strId);
        }

        public SyntaxToken MarkToken(SyntaxToken token)
        {
            var strId = RoslynCompiler.UniqueId();
            return RoslynCompiler.MarkToken(token, strId);
        }

        public SyntaxToken InitToken(SyntaxToken token, int xsId)
        {
            return RoslynCompiler.MarkToken(token, xsId.ToString());
        }

        public IEnumerable<SyntaxToken> MarkTokens(IEnumerable<SyntaxToken> tokens, out int xsId)
        {
            var strId = RoslynCompiler.UniqueId();
            xsId = int.Parse(strId);

            return tokens.Select(token => RoslynCompiler.MarkToken(token, strId));
        }

        public SyntaxNode MarkNode(SyntaxNode node, out int xsId)
        {
            var strId = GetExcessStringId(node);
            if (strId != null)
            {
                xsId = int.Parse(strId);
                return node;
            }

            strId = RoslynCompiler.UniqueId();
            xsId = int.Parse(strId);
            return RoslynCompiler.MarkNode(node, strId);
        }

        public SyntaxNode MarkNode(SyntaxNode node)
        {
            return RoslynCompiler.MarkNode(node, RoslynCompiler.UniqueId()); //TODO: scope ids
        }

        public SyntaxNode MarkTree(SyntaxNode node)
        {
            //TODO: optimize?
            return node.ReplaceNodes(node.DescendantNodes(), (oldNode, newNode) => MarkNode(newNode));
        }

        public int GetExcessId(SyntaxToken token)
        {
            var xsStr = RoslynCompiler.TokenMark(token);
            return xsStr == null
                ? -1
                : int.Parse(xsStr);
        }

        public int GetExcessId(SyntaxNode node)
        {
            var xsStr = RoslynCompiler.NodeMark(node);
            return xsStr == null
                ? -1
                : int.Parse(xsStr);
        }

        public bool IsIdentifier(SyntaxToken token)
        {
            return RoslynCompiler.IsLexicalIdentifier(token.Kind());
        }


        public SyntaxNode Parse(string text)
        {
            return SyntaxFactory.ParseSyntaxTree(text).GetRoot();
        }

        public IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            return SyntaxFactory.ParseTokens(text)
                .Where(token => token.Kind() != SyntaxKind.EndOfFileToken);
        }

        public IEnumerable<SyntaxNode> Find(SyntaxNode node, IEnumerable<string> xsIds)
        {
            //TODO: optimize
            foreach (var id in xsIds)
            {
                var result = FindNode(node, id);
                if (result != null)
                    yield return result;
            }
        }

        public SyntaxNode Find(SyntaxNode node, SourceSpan span)
        {
            return node.FindNode(new TextSpan(span.Start, span.Length));
        }

        public int GetOffset(SyntaxToken token)
        {
            return token.SpanStart;
        }

        public string GetExcessStringId(SyntaxNode node)
        {
            return RoslynCompiler.NodeMark(node);
        }

        public SyntaxNode FindNode(SyntaxNode node, string xsId)
        {
            return node.GetAnnotatedNodes(RoslynCompiler.NodeIdAnnotation + xsId).FirstOrDefault();
        }
    }
}