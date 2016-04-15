using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Excess.Compiler.Roslyn
{
    public class MappingService : IMappingService<SyntaxNode>
    {
        private readonly List<Change> _changes = new List<Change>();
        private readonly int _currOriginal = 0;

        private readonly Dictionary<SyntaxNode, SyntaxNode> _map = new Dictionary<SyntaxNode, SyntaxNode>();
        public SyntaxNode LexicalTree { get; set; }
        public SyntaxNode SyntacticalTree { get; set; }
        public SyntaxNode SemanticalTree { get; set; }

        public void LexicalChange(SourceSpan oldSpan, int newLength)
        {
            _changes.Add(new Change
            {
                Original = oldSpan.Start - _currOriginal,
                Modified = oldSpan.Start - _currOriginal,
                IsChange = false
            });

            _changes.Add(new Change {Original = oldSpan.Length, Modified = newLength, IsChange = true});
        }

        public void SemanticalChange(SyntaxNode oldNode, SyntaxNode newNode)
        {
            _map[oldNode] = newNode;

            var oldNodes = oldNode.DescendantNodes().GetEnumerator();
            var newNodes = newNode.DescendantNodes().GetEnumerator();

            while (true)
            {
                if (!oldNodes.MoveNext())
                {
                    break;
                }

                if (!newNodes.MoveNext())
                {
                    break;
                }

                _map[oldNodes.Current] = newNodes.Current;
            }
        }

        public SyntaxNode SemanticalMap(SyntaxNode node)
        {
            return _map[node];
        }

        public SyntaxNode SemanticalMap(SourceSpan src)
        {
            if (LexicalTree == null)
            {
                throw new InvalidOperationException("LexicalTree");
            }

            if (SyntacticalTree == null)
            {
                throw new InvalidOperationException("SyntacticalTree");
            }

            var lexSpan = MapSource(src);

            var node = LexicalTree.FindNode(lexSpan, getInnermostNodeForTie: true);
            if (node == null)
            {
                return null;
            }

            node = MapLexical(node);
            if (node == null)
            {
                return null;
            }

            if (SemanticalTree != null)
            {
                return _map[node];
            }

            return node;
        }

        public SourceSpan SourceMap(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        private SyntaxNode MapLexical(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        private TextSpan MapSource(SourceSpan src)
        {
            throw new NotImplementedException();
        }

        private struct Change
        {
            public int Original;
            public int Modified;
            public bool IsChange;
        }
    }
}