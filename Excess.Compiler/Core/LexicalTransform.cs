using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public class LexicalTransform<TToken, TNode, TModel> : ILexicalTransform<TToken, TNode, TModel>
    {
        private readonly List<Transformer> _transformers = new List<Transformer>();
        private string _refToken;
        private Func<TNode, TNode, TModel, Scope, TNode> _semantical;
        private Func<TNode, Scope, TNode> _syntactical;

        public ILexicalTransform<TToken, TNode, TModel> Insert(string tokenString, string before = null, string after = null)
        {
            if (before != null)
            {
                AddTransformer(before, (tokens, scope) => TokensFromString(tokenString, scope).Union(tokens), 0);
            }
            else if (after != null)
            {
                AddTransformer(after, (tokens, scope) => tokens.Union(TokensFromString(tokenString, scope)), 0);
            }
            else
            {
                throw new InvalidOperationException("Must specify either 'after' or 'before'");
            }

            return this;
        }

        public ILexicalTransform<TToken, TNode, TModel> Replace(string named, string tokenString)
        {
            if (named != null)
            {
                AddTransformer(named, (tokens, scope) => TokensFromString(tokenString, scope), -1);
            }
            else
            {
                throw new InvalidOperationException("Must specify 'named'");
            }

            return this;
        }

        public ILexicalTransform<TToken, TNode, TModel> Remove(string named)
        {
            if (named != null)
            {
                AddTransformer(named, (tokens, scope) => new TToken[] {}, -1);
            }
            else
            {
                throw new InvalidOperationException("Must specify 'named'");
            }

            return this;
        }

        public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode> handler, string referenceToken = null)
        {
            return Then((node, scope) => handler(node), referenceToken);
        }

        public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler, string referenceToken = null)
        {
            Debug.Assert(_syntactical == null && _semantical == null);
            _refToken = referenceToken;
            _syntactical = handler;

            return this;
        }

        public ILexicalTransform<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler, string referenceToken = null)
        {
            Debug.Assert(_syntactical == null && _semantical == null);
            _refToken = referenceToken;
            _semantical = handler;

            return this;
        }

        public IEnumerable<TToken> Transform(IEnumerable<TToken> tokens, ILexicalMatchResult<TToken, TNode, TModel> match, Scope scope)
        {
            var sorted = _transformers.OrderBy(t => t.Priority);
            var compiler = scope.GetService<TToken, TNode, TModel>();
            var needsMark = _syntactical != null;
            var id = -1;

            foreach (var item in match.Items)
            {
                var result = match.GetTokens(tokens, item.Span);
                foreach (var transformer in sorted)
                {
                    if (transformer.Item == item.Identifier)
                    {
                        result = transformer.Handler(result, scope);
                    }
                }

                if (_refToken != null)
                {
                    needsMark = item.Identifier == _refToken;
                }

                foreach (var token in result)
                {
                    if (needsMark)
                    {
                        TToken marked;
                        if (id < 0)
                        {
                            var document = scope.GetDocument<TToken, TNode, TModel>();
                            if (_syntactical != null)
                            {
                                marked = document.Change(token, _syntactical);
                            }
                            else
                            {
                                Debug.Assert(_semantical != null);
                                marked = document.Change(token, _semantical);
                            }

                            id = compiler.GetExcessId(marked);
                        }
                        else
                        {
                            marked = compiler.InitToken(token, id);
                        }

                        yield return marked;
                    }
                    else
                    {
                        yield return token;
                    }
                }
            }
        }

        private IEnumerable<TToken> TokensFromString(string tokenString, Scope scope)
        {
            var compiler = scope.GetService<TToken, TNode, TModel>();
            return compiler.ParseTokens(tokenString);
        }

        private IEnumerable<TToken> EmptyTokens(IEnumerable<TToken> tokens, Scope scope)
        {
            return new TToken[] {};
        }

        private void AddTransformer(string target, Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> handler, int priority)
        {
            _transformers.Add(new Transformer
            {
                Item = target,
                Handler = handler,
                Priority = priority
            });
        }

        private class Transformer
        {
            public string Item { get; set; }
            public Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> Handler { get; set; }
            public int Priority { get; set; }
        }
    }
}