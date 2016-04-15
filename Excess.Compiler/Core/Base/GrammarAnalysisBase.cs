using System;
using System.Collections.Generic;
using System.Linq;

namespace Excess.Compiler.Core
{
    public class BaseGrammarAnalysis<TToken, TNode, TModel, TGNode, TGrammar> : IGrammarAnalysis<TGrammar, TGNode, TToken, TNode>
        where TGrammar : IGrammar<TToken, TNode, TGNode>, new()
    {
        private readonly TGrammar _grammar;

        private readonly Dictionary<Type, Func<TGNode, Func<TGNode, Scope, TNode>, Scope, TNode>> _transformers =
            new Dictionary<Type, Func<TGNode, Func<TGNode, Scope, TNode>, Scope, TNode>>();

        private Func<TNode, TNode, Scope, LexicalExtensionDto<TToken>, TNode> _then;

        public BaseGrammarAnalysis(ILexicalAnalysis<TToken, TNode, TModel> lexical, string keyword, ExtensionKind kind)
        {
            lexical.Extension(keyword, kind, ParseExtension);
            _grammar = new TGrammar();
        }

        public void Then(Func<TNode, TNode, Scope, LexicalExtensionDto<TToken>, TNode> handler)
        {
            _then = handler;
        }

        public IGrammarAnalysis<TGrammar, TGNode, TToken, TNode> Transform<T>(Func<T, Func<TGNode, Scope, TNode>, Scope, TNode> handler) where T : TGNode
        {
            var type = typeof(T);
            if (_transformers.ContainsKey(type))
            {
                throw new InvalidOperationException("multiple type handlers");
            }

            _transformers[type] = (node, grammar, scope) => handler((T) node, DoTransform, scope);
            return this;
        }

        private TNode ParseExtension(TNode node, Scope scope, LexicalExtensionDto<TToken> extension)
        {
            var allTokens = extension.Body.ToArray();
            var woBraces = Range(allTokens, 1, allTokens.Length - 1);
            var withoutBraces = woBraces as TToken[] ?? woBraces.ToArray();
            if (!withoutBraces.Any())
            {
                return node;
            }

            var compiler = scope.GetService<TToken, TNode, TModel>();
            var g = _grammar.Parse(withoutBraces, scope, compiler.GetOffset(withoutBraces.First()));

            if (g == null || g.Equals(default(TGNode)))
            {
                return node; //errors added to the scope already
            }

            var result = DoTransform(g, scope);
            if (_then != null)
            {
                result = _then(node, result, scope, extension);
            }

            return result;
        }

        private IEnumerable<TToken> Range(TToken[] tokens, int from, int to)
        {
            for (var i = from; i < to; i++)
            {
                yield return tokens[i];
            }
        }

        private TNode DoTransform(TGNode tg, Scope scope)
        {
            var type = tg.GetType();
            Func<TGNode, Func<TGNode, Scope, TNode>, Scope, TNode> handler;
            if (_transformers.TryGetValue(type, out handler))
            {
                return handler(tg, DoTransform, scope);
            }

            return default(TNode);
        }
    }
}