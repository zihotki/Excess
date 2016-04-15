using System;
using System.Collections.Generic;

namespace Excess.Compiler.Core
{
    public class BaseNormalizer<TToken, TNode, TModel> : INormalizer<TToken, TNode, TModel>
    {
        private readonly BaseLexicalAnalysis<TToken, TNode, TModel> _owner;

        public BaseNormalizer(BaseLexicalAnalysis<TToken, TNode, TModel> owner)
        {
            _owner = owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> With(
            Func<TNode, IEnumerable<TNode>, Scope, TNode> statements = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> members = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> types = null,
            Func<TNode, Scope, TNode> then = null)
        {
            _owner.Normalize(statements, members, types, then);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Statements(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.Normalize(handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Members(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.Normalize(members: handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Types(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.Normalize(types: handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler)
        {
            _owner.Normalize(then: handler);
            return _owner;
        }
    }
}