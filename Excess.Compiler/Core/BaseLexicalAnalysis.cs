using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public abstract class BaseLexicalAnalysis<TToken, TNode, TModel> : ILexicalAnalysis<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        private readonly List<ILexicalMatch<TToken, TNode, TModel>> _matchers = new List<ILexicalMatch<TToken, TNode, TModel>>();
        private Dictionary<int, Func<Scope, LexicalExtensionDto<TToken>, TNode>> _extensions = new Dictionary<int, Func<Scope, LexicalExtensionDto<TToken>, TNode>>();
        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> NormalizeMembers;

        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> NormalizeStatements;
        protected Func<TNode, Scope, TNode> NormalizeThen;
        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> NormalizeTypes;

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            document.Change(LexicalPass);

            if (NormalizeStatements != null || NormalizeMembers != null || NormalizeTypes != null)
            {
                document.Change(Normalize, "normalize");
            }
        }

        public INormalizer<TToken, TNode, TModel> Normalize()
        {
            return new BaseNormalizer<TToken, TNode, TModel>(this);
        }

        public ILexicalMatch<TToken, TNode, TModel> Match()
        {
            var result = CreateMatch();
            _matchers.Add(result);
            return result;
        }

        public virtual ILexicalTransform<TToken, TNode, TModel> Transform()
        {
            return new LexicalTransform<TToken, TNode, TModel>();
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, Scope, LexicalExtensionDto<TToken>, TNode> handler)
        {
            return Extension(keyword, kind, SyntacticalExtension(handler));
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind,
            Func<IEnumerable<TToken>, Scope, LexicalExtensionDto<TToken>, IEnumerable<TToken>> handler)
        {
            var result = CreateMatch();

            result
                .Token(keyword, "keyword")
                .Identifier("id", true)
                .Enclosed('(', ')', contents: "arguments")
                .Enclosed('{', '}', contents: "body")
                .Then(ReplaceExtension(keyword, kind, handler));

            _matchers.Add(result);
            return this;
        }

        public IGrammarAnalysis<TGrammar, TGNode, TToken, TNode> Grammar<TGrammar, TGNode>(string keyword, ExtensionKind kind)
            where TGrammar : IGrammar<TToken, TNode, TGNode>, new()
        {
            return new BaseGrammarAnalysis<TToken, TNode, TModel, TGNode, TGrammar>(this, keyword, kind);
        }

        protected abstract TNode Normalize(TNode node, Scope scope);

        private IEnumerable<TToken> LexicalPass(IEnumerable<TToken> tokens, Scope scope)
        {
            var allTokens = tokens.ToArray();
            return TransformSpan(allTokens, new TokenSpanDto(0, allTokens.Length), scope);
        }

        private IEnumerable<TToken> TransformSpan(TToken[] tokens, TokenSpanDto span, Scope scope)
        {
            var builders = new List<MatchInfo>();

            var currentToken = 0;
            while (currentToken < span.Length)
            {
                var remaining = Range(tokens, span.Start + currentToken, span.Length - currentToken);

                ILexicalMatchResult<TToken, TNode, TModel> result = null;
                foreach (var matcher in _matchers)
                {
                    result = matcher.Match(remaining, scope, currentToken == 0);
                    if (result != null)
                    {
                        break;
                    }
                }

                if (result != null)
                {
                    builders.Add(new MatchInfo {Span = new TokenSpanDto(span.Start + currentToken, result.Consumed), Match = result});
                    currentToken += result.Consumed;
                }
                else
                {
                    currentToken++;
                }
            }

            var returnValue = TransformBuilders(tokens, span, builders, scope);
            return returnValue;
        }

        private IEnumerable<TToken> TransformBuilders(TToken[] tokens, TokenSpanDto span, List<MatchInfo> builders, Scope scope)
        {
            var current = span.Start;
            foreach (var builder in builders)
            {
                if (builder.Span.Start > current)
                {
                    for (var i = current; i < builder.Span.Start; i++)
                    {
                        yield return tokens[i]; //literal
                    }

                    current = builder.Span.Start;
                }

                int consumed;
                var matchedTokens = BuildMatchResult(tokens, builder, scope, out consumed);
                var transformer = builder.Match.Transform;

                current += consumed;
                Debug.Assert(transformer != null);
                var matchTokens = transformer.Transform(matchedTokens, builder.Match, scope);
                foreach (var matchToken in matchTokens)
                {
                    yield return matchToken;
                }
            }

            for (var i = current; i < span.Start + span.Length; i++)
            {
                yield return tokens[i]; //finish
            }
        }

        private IEnumerable<TToken> BuildMatchResult(TToken[] tokens, MatchInfo builder, Scope scope, out int consumed)
        {
            var resultItems = new List<LexicalMatchItemDto>();
            var resultTokens = new List<TToken>();

            var start = builder.Span.Start;
            foreach (var item in builder.Match.Items)
            {
                var newItem = new LexicalMatchItemDto(new TokenSpanDto(resultTokens.Count, item.Span.Length), item.Identifier, item.Literal);
                if (item.Span.Length > 0)
                {
                    var span = new TokenSpanDto(start + item.Span.Start, item.Span.Length);
                    if (item.Span.Length > 1)
                    {
                        var itemTokens = !item.Literal
                            ? TransformSpan(tokens, span, scope)
                            : Range(tokens, span);

                        var oldCount = resultTokens.Count;
                        resultTokens.AddRange(itemTokens);

                        newItem.Span.Length = resultTokens.Count - oldCount;
                    }
                    else
                    {
                        resultTokens.Add(tokens[span.Start]);
                    }
                }

                resultItems.Add(newItem);
            }

            builder.Match = new BaseLexicalMatchResult<TToken, TNode, TModel>(resultItems, builder.Match.Transform);
            consumed = builder.Span.Length;
            return resultTokens;
        }

        private IEnumerable<TToken> Range(TToken[] tokens, TokenSpanDto span)
        {
            return Range(tokens, span.Start, span.Length);
        }

        private IEnumerable<TToken> Range(IReadOnlyList<TToken> tokens, int start, int length)
        {
            for (var i = 0; i < length; i++)
            {
                yield return tokens[start + i];
            }
        }

        private ILexicalMatch<TToken, TNode, TModel> CreateMatch()
        {
            return new BaseLexicalMatch<TToken, TNode, TModel>(this);
        }

        public void Normalize(Func<TNode, IEnumerable<TNode>, Scope, TNode> statements = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> members = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> types = null,
            Func<TNode, Scope, TNode> then = null)
        {
            if (statements != null)
            {
                NormalizeStatements = statements; //overrides
            }

            if (members != null)
            {
                NormalizeMembers = members;
            }

            if (types != null)
            {
                NormalizeTypes = types;
            }

            if (then != null)
            {
                NormalizeThen = then;
            }
        }

        private Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> ReplaceExtension(string keyword, ExtensionKind kind,
            Func<IEnumerable<TToken>, Scope, LexicalExtensionDto<TToken>, IEnumerable<TToken>> handler)
        {
            return (tokens, scope) =>
            {
                dynamic context = scope;

                var extension = new LexicalExtensionDto<TToken>
                {
                    Kind = kind,
                    Keyword = context.keyword,
                    Identifier = context.id ?? default(TToken),
                    Arguments = context.arguments,
                    Body = context.body
                };

                return handler(tokens, scope, extension);
            };
        }

        private Func<IEnumerable<TToken>, Scope, LexicalExtensionDto<TToken>, IEnumerable<TToken>> SyntacticalExtension(
            Func<TNode, Scope, LexicalExtensionDto<TToken>, TNode> handler)
        {
            return (tokens, scope, extension) =>
            {
                var compiler = scope.GetService<TToken, TNode, TModel>();

                //insert some placeholders, depending on the extension kind
                switch (extension.Kind)
                {
                    case ExtensionKind.Code:
                    {
                        tokens = compiler.ParseTokens("__extension();");
                        break;
                    }
                    case ExtensionKind.Member:
                    {
                        tokens = compiler.ParseTokens("void __extension() {}");
                        break;
                    }
                    case ExtensionKind.Type:
                    {
                        tokens = compiler.ParseTokens("class __extension() {}");
                        break;
                    }

                    default:
                        throw new InvalidOperationException();
                }

                //schedule the processing of these extensions for a time we actally have syntax
                var document = scope.GetDocument<TToken, TNode, TModel>();
                return document.Change(tokens, TransformLexicalExtension(extension, handler), "lexical-extension");
            };
        }

        private Func<TNode, Scope, TNode> TransformLexicalExtension(LexicalExtensionDto<TToken> extension,
            Func<TNode, Scope, LexicalExtensionDto<TToken>, TNode> handler)
        {
            return (node, scope) => handler(node, scope, extension);
        }

        private class MatchInfo
        {
            public TokenSpanDto Span { get; set; }
            public ILexicalMatchResult<TToken, TNode, TModel> Match { get; set; }
        }
    }
}