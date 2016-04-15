using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Excess.Compiler.Core
{
    public class BaseLexicalMatch<TToken, TNode, TModel> : ILexicalMatch<TToken, TNode, TModel>
    {
        private readonly ILexicalAnalysis<TToken, TNode, TModel> _lexical;
        private readonly List<Func<MatchResultBuilder, Scope, bool>> _matchers = new List<Func<MatchResultBuilder, Scope, bool>>();
        private ILexicalTransform<TToken, TNode, TModel> _transform;

        public BaseLexicalMatch(ILexicalAnalysis<TToken, TNode, TModel> lexical)
        {
            _lexical = lexical;
        }

        public ILexicalMatchResult<TToken, TNode, TModel> Match(IEnumerable<TToken> tokens, Scope scope, bool isDocumentStart)
        {
            var builder = new MatchResultBuilder(tokens, _transform, isDocumentStart);
            var inner = new Scope(scope);
            foreach (var matcher in _matchers)
            {
                if (!matcher(builder, inner))
                {
                    return null;
                }
            }

            return builder.GetResult();
        }

        public ILexicalMatch<TToken, TNode, TModel> Enclosed(string open, string close, string start, string end, string contents)
        {
            return Enclosed(MatchString(open), MatchString(close), start, end, contents);
        }

        public ILexicalMatch<TToken, TNode, TModel> Enclosed(char open, char close, string start = null, string end = null, string contents = null)
        {
            return Enclosed(MatchString(open.ToString()), MatchString(close.ToString()), start, end, contents);
        }

        public ILexicalMatch<TToken, TNode, TModel> Enclosed(Func<TToken, bool> open, Func<TToken, bool> close, string start = null, string end = null,
            string contents = null)
        {
            _matchers.Add(MatchEnclosed(open, close, start, end, contents));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Many(params string[] anyOf)
        {
            return Many(MatchStringArray(anyOf));
        }

        public ILexicalMatch<TToken, TNode, TModel> Many(params char[] anyOf)
        {
            return Many(MatchStringArray(anyOf.Select(ch => ch.ToString())));
        }

        public ILexicalMatch<TToken, TNode, TModel> Many(string[] anyOf, string named = null)
        {
            return Many(MatchStringArray(anyOf), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Many(char[] anyOf, string named = null)
        {
            return Many(MatchStringArray(anyOf.Select(ch => ch.ToString())), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Many(Func<TToken, bool> tokens, string named = null)
        {
            _matchers.Add(MatchMany(tokens, named));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> ManyOrNone(params string[] anyOf)
        {
            return ManyOrNone(MatchStringArray(anyOf));
        }

        public ILexicalMatch<TToken, TNode, TModel> ManyOrNone(params char[] anyOf)
        {
            return ManyOrNone(MatchStringArray(anyOf.Select(ch => ch.ToString())));
        }

        public ILexicalMatch<TToken, TNode, TModel> ManyOrNone(string[] anyOf, string named = null)
        {
            return ManyOrNone(MatchStringArray(anyOf), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> ManyOrNone(char[] anyOf, string named = null)
        {
            return ManyOrNone(MatchStringArray(anyOf.Select(ch => ch.ToString())), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> ManyOrNone(Func<TToken, bool> tokens, string named = null)
        {
            _matchers.Add(MatchMany(tokens, named, true));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Token(char value, string named = null)
        {
            return Token(MatchString(value.ToString()), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Token(string value, string named = null)
        {
            return Token(MatchString(value), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Token(Func<TToken, bool> matcher, string named = null)
        {
            _matchers.Add(MatchOne(matcher, named));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Any(params string[] anyOf)
        {
            return Any(MatchStringArray(anyOf));
        }

        public ILexicalMatch<TToken, TNode, TModel> Any(params char[] anyOf)
        {
            return Any(MatchStringArray(anyOf.Select(ch => ch.ToString())));
        }

        public ILexicalMatch<TToken, TNode, TModel> Any(string[] anyOf, string named = null, bool matchDocumentStart = false)
        {
            return Any(MatchStringArray(anyOf), named, matchDocumentStart);
        }

        public ILexicalMatch<TToken, TNode, TModel> Any(char[] anyOf, string named = null, bool matchDocumentStart = false)
        {
            return Any(MatchStringArray(anyOf.Select(ch => ch.ToString())), named, matchDocumentStart);
        }

        public ILexicalMatch<TToken, TNode, TModel> Any(Func<TToken, bool> handler, string named = null, bool matchDocumentStart = false)
        {
            _matchers.Add(MatchOne(handler, named, matchDocumentStart: matchDocumentStart));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Optional(params string[] anyOf)
        {
            return Any(MatchStringArray(anyOf));
        }

        public ILexicalMatch<TToken, TNode, TModel> Optional(params char[] anyOf)
        {
            return Any(MatchStringArray(anyOf.Select(ch => ch.ToString())));
        }

        public ILexicalMatch<TToken, TNode, TModel> Optional(string[] anyOf, string named = null)
        {
            return Any(MatchStringArray(anyOf), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Optional(char[] anyOf, string named = null)
        {
            return Any(MatchStringArray(anyOf.Select(ch => ch.ToString())), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Optional(Func<TToken, bool> handler, string named = null)
        {
            _matchers.Add(MatchOne(handler, named, true));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Identifier(string named = null, bool optional = false)
        {
            _matchers.Add(MatchIdentifier(named, optional));
            return this;
        }

        public ILexicalMatch<TToken, TNode, TModel> Until(char token, string named = null)
        {
            return Until(MatchString(token.ToString()), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Until(string token, string named = null)
        {
            return Until(MatchString(token), named);
        }

        public ILexicalMatch<TToken, TNode, TModel> Until(Func<TToken, bool> matcher, string named = null)
        {
            _matchers.Add(MatchUntil(matcher, named));
            return this;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler)
        {
            return Then(new LexicalFunctorTransform<TToken, TNode, TModel>(ScheduleSyntactical(handler)));
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler)
        {
            return Then(new LexicalFunctorTransform<TToken, TNode, TModel>(ScheduleSemantical(handler)));
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> handler)
        {
            _transform = new LexicalFunctorTransform<TToken, TNode, TModel>(handler);
            return _lexical;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(ILexicalTransform<TToken, TNode, TModel> transform)
        {
            _transform = transform;
            return _lexical;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(
            Func<IEnumerable<TToken>, ILexicalMatchResult<TToken, TNode, TModel>, Scope, IEnumerable<TToken>> handler)
        {
            _transform = new LexicalFunctorTransform<TToken, TNode, TModel>(handler);
            return _lexical;
        }


        //matchers
        private static Func<MatchResultBuilder, Scope, bool> MatchEnclosed(Func<TToken, bool> open, Func<TToken, bool> close, string start, string end,
            string contents)
        {
            return (match, scope) =>
            {
                if (!open(match.Peek()))
                {
                    return false;
                }

                var depthCount = 1;
                if (start != null)
                {
                    match.AddResult(1, start);
                }
                else
                {
                    depthCount = 0; //expect an open token
                }

                var contentLength = 0;
                foreach (var token in match.Remaining())
                {
                    if (open(token))
                    {
                        depthCount++;
                    }
                    else if (close(token))
                    {
                        depthCount--;
                        if (depthCount == 0)
                        {
                            if (end != null)
                            {
                                match.AddResult(contentLength, contents);
                                match.AddResult(1, end);
                            }
                            else
                            {
                                match.AddResult(contentLength + 1, contents);
                            }

                            return true;
                        }
                    }

                    contentLength++;
                }

                return false;
            };
        }

        private static Func<TToken, bool> MatchString(string value)
        {
            return token => token.ToString() == value;
        }

        private static Func<MatchResultBuilder, Scope, bool> MatchMany(Func<TToken, bool> selector, string named, bool matchNone = false, bool literal = false)
        {
            return (match, scope) =>
            {
                var matched = 0;
                while (match.Any())
                {
                    if (!selector(match.Take()))
                    {
                        break;
                    }

                    matched++;
                }

                if (matched == 0 && !matchNone)
                {
                    return false;
                }

                match.AddResult(matched, named, literal);
                return true;
            };
        }

        private static Func<TToken, bool> MatchStringArray(IEnumerable<string> values)
        {
            return token =>
            {
                var tokenValue = token.ToString();
                foreach (var value in values)
                {
                    if (tokenValue == value)
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        private static Func<MatchResultBuilder, Scope, bool> MatchOne(Func<TToken, bool> selector, string named = null, bool matchNone = false,
            bool matchDocumentStart = false)
        {
            return (match, scope) =>
            {
                if (selector(match.Peek()))
                {
                    match.AddResult(1, named);
                    return true;
                }

                if (matchNone || (matchDocumentStart && match.isDocumentStart()))
                {
                    match.AddResult(0, named);
                    return true;
                }

                return false;
            };
        }

        private static Func<MatchResultBuilder, Scope, bool> MatchIdentifier(string named = null, bool matchNone = false, bool matchDocumentStart = false)
        {
            return (match, scope) =>
            {
                var compiler = scope.GetService<TToken, TNode, TModel>();
                if (compiler.IsIdentifier(match.Peek()))
                {
                    match.AddResult(1, named);
                    return true;
                }

                if (matchNone || (matchDocumentStart && match.isDocumentStart()))
                {
                    match.AddResult(0, named);
                    return true;
                }

                return false;
            };
        }

        private static Func<MatchResultBuilder, Scope, bool> MatchUntil(Func<TToken, bool> selector, string named = null, string matchNamed = null)
        {
            return (match, scope) =>
            {
                var contentLength = 0;
                foreach (var token in match.Remaining())
                {
                    if (selector(token))
                    {
                        if (matchNamed != null)
                        {
                            match.AddResult(contentLength, named);
                            match.AddResult(1, matchNamed);
                        }
                        else
                        {
                            match.AddResult(contentLength + 1, named);
                        }

                        return true;
                    }

                    contentLength++;
                }

                return false;
            };
        }

        private Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> ScheduleSyntactical(Func<TNode, Scope, TNode> handler)
        {
            return (tokens, scope) =>
            {
                var document = scope.GetDocument<TToken, TNode, TModel>();
                return document.Change(tokens, handler);
            };
        }

        private Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> ScheduleSemantical(Func<TNode, TNode, TModel, Scope, TNode> handler)
        {
            return (tokens, scope) =>
            {
                var document = scope.GetDocument<TToken, TNode, TModel>();
                return document.Change(tokens, handler);
            };
        }

        private class MatchResultBuilder
        {
            private readonly bool _isDocumentStart;

            private readonly List<LexicalMatchItem> _items = new List<LexicalMatchItem>();
            private readonly ILexicalTransform<TToken, TNode, TModel> _transform;
            private int _current;

            private IEnumerable<TToken> _tokens;

            public MatchResultBuilder(IEnumerable<TToken> tokens, ILexicalTransform<TToken, TNode, TModel> transform, bool isDocumentStart)
            {
                _tokens = tokens;
                _transform = transform;
                _isDocumentStart = isDocumentStart;
            }

            public TToken Peek()
            {
                return _tokens.FirstOrDefault();
            }

            public IEnumerable<TToken> Remaining()
            {
                return _tokens;
            }

            public TokenSpan Consume(int count)
            {
                var result = new TokenSpan(_current, count);
                _current += count;
                _tokens = _tokens.Skip(count);
                return result;
            }

            public void AddResult(int length, string identifier, bool literal = false)
            {
                _items.Add(new LexicalMatchItem(Consume(length), identifier, literal));
            }

            public bool isDocumentStart()
            {
                return _isDocumentStart && _current == 0;
            }

            public bool Any()
            {
                return _tokens.Any();
            }

            internal TToken Take()
            {
                _current++;
                var result = _tokens.First();
                _tokens = _tokens.Skip(1);
                return result;
            }

            internal ILexicalMatchResult<TToken, TNode, TModel> GetResult()
            {
                return new BaseLexicalMatchResult<TToken, TNode, TModel>(_items, _transform);
            }
        }
    }

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
            _owner.normalize(statements, members, types, then);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Statements(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.normalize(handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Members(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.normalize(members: handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Types(Func<TNode, IEnumerable<TNode>, Scope, TNode> handler)
        {
            _owner.normalize(types: handler);
            return _owner;
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler)
        {
            _owner.normalize(then: handler);
            return _owner;
        }
    }

    public abstract class BaseLexicalAnalysis<TToken, TNode, TModel> : ILexicalAnalysis<TToken, TNode, TModel>,
        IDocumentInjector<TToken, TNode, TModel>
    {
        private readonly List<ILexicalMatch<TToken, TNode, TModel>> _matchers = new List<ILexicalMatch<TToken, TNode, TModel>>();
        private Dictionary<int, Func<Scope, LexicalExtension<TToken>, TNode>> _extensions = new Dictionary<int, Func<Scope, LexicalExtension<TToken>, TNode>>();
        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> _normalizeMembers;

        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> _normalizeStatements;
        protected Func<TNode, Scope, TNode> _normalizeThen;
        protected Func<TNode, IEnumerable<TNode>, Scope, TNode> _normalizeTypes;

        public void Apply(IDocument<TToken, TNode, TModel> document)
        {
            document.Change(LexicalPass);

            if (_normalizeStatements != null || _normalizeMembers != null || _normalizeTypes != null)
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
            var result = createMatch();
            _matchers.Add(result);
            return result;
        }

        public virtual ILexicalTransform<TToken, TNode, TModel> Transform()
        {
            return new LexicalTransform<TToken, TNode, TModel>();
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind, Func<TNode, Scope, LexicalExtension<TToken>, TNode> handler)
        {
            return Extension(keyword, kind, SyntacticalExtension(handler));
        }

        public ILexicalAnalysis<TToken, TNode, TModel> Extension(string keyword, ExtensionKind kind,
            Func<IEnumerable<TToken>, Scope, LexicalExtension<TToken>, IEnumerable<TToken>> handler)
        {
            var result = createMatch();

            result
                .Token(keyword, "keyword")
                .Identifier("id", true)
                .Enclosed('(', ')', contents: "arguments")
                .Enclosed('{', '}', contents: "body")
                .Then(ReplaceExtension(keyword, kind, handler));

            _matchers.Add(result);
            return this;
        }

        public IGrammarAnalysis<TGrammar, GNode, TToken, TNode> Grammar<TGrammar, GNode>(string keyword, ExtensionKind kind)
            where TGrammar : IGrammar<TToken, TNode, GNode>, new()
        {
            return new BaseGrammarAnalysis<TToken, TNode, TModel, GNode, TGrammar>(this, keyword, kind);
        }

        protected abstract TNode Normalize(TNode node, Scope scope);

        private IEnumerable<TToken> LexicalPass(IEnumerable<TToken> tokens, Scope scope)
        {
            var allTokens = tokens.ToArray();
            return TransformSpan(allTokens, new TokenSpan(0, allTokens.Length), scope);
        }

        private IEnumerable<TToken> TransformSpan(TToken[] tokens, TokenSpan span, Scope scope)
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
                    builders.Add(new MatchInfo {Span = new TokenSpan(span.Start + currentToken, result.Consumed), Match = result});
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

        private IEnumerable<TToken> TransformBuilders(TToken[] tokens, TokenSpan span, List<MatchInfo> builders, Scope scope)
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
                var matchedTokens = buildMatchResult(tokens, builder, scope, out consumed);
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

        private IEnumerable<TToken> buildMatchResult(TToken[] tokens, MatchInfo builder, Scope scope, out int consumed)
        {
            var resultItems = new List<LexicalMatchItem>();
            var resultTokens = new List<TToken>();

            var start = builder.Span.Start;
            foreach (var item in builder.Match.Items)
            {
                var newItem = new LexicalMatchItem(new TokenSpan(resultTokens.Count, item.Span.Length), item.Identifier, item.Literal);
                if (item.Span.Length > 0)
                {
                    var span = new TokenSpan(start + item.Span.Start, item.Span.Length);
                    if (item.Span.Length > 1)
                    {
                        IEnumerable<TToken> itemTokens;
                        if (!item.Literal)
                        {
                            itemTokens = TransformSpan(tokens, span, scope);
                        }
                        else
                        {
                            itemTokens = Range(tokens, span);
                        }

                        var oldCount = resultTokens.Count;
                        resultTokens.AddRange(itemTokens);
                        var newCount = itemTokens.Count();

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

        private IEnumerable<TToken> Range(TToken[] tokens, TokenSpan span)
        {
            return Range(tokens, span.Start, span.Length);
        }

        private IEnumerable<TToken> Range(TToken[] tokens, int start, int length)
        {
            for (var i = 0; i < length; i++)
            {
                yield return tokens[start + i];
            }
        }

        private ILexicalMatch<TToken, TNode, TModel> createMatch()
        {
            return new BaseLexicalMatch<TToken, TNode, TModel>(this);
        }

        public void normalize(Func<TNode, IEnumerable<TNode>, Scope, TNode> statements = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> members = null,
            Func<TNode, IEnumerable<TNode>, Scope, TNode> types = null,
            Func<TNode, Scope, TNode> then = null)
        {
            if (statements != null)
            {
                _normalizeStatements = statements; //overrides
            }

            if (members != null)
            {
                _normalizeMembers = members;
            }

            if (types != null)
            {
                _normalizeTypes = types;
            }

            if (then != null)
            {
                _normalizeThen = then;
            }
        }

        private Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> ReplaceExtension(string keyword, ExtensionKind kind,
            Func<IEnumerable<TToken>, Scope, LexicalExtension<TToken>, IEnumerable<TToken>> handler)
        {
            return (tokens, scope) =>
            {
                dynamic context = scope;

                var extension = new LexicalExtension<TToken>
                {
                    Kind = kind,
                    Keyword = context.keyword,
                    Identifier = context.id != null
                        ? context.id
                        : default(TToken),
                    Arguments = context.arguments,
                    Body = context.body
                };

                return handler(tokens, scope, extension);
            };
        }

        private Func<IEnumerable<TToken>, Scope, LexicalExtension<TToken>, IEnumerable<TToken>> SyntacticalExtension(
            Func<TNode, Scope, LexicalExtension<TToken>, TNode> handler)
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

        private Func<TNode, Scope, TNode> TransformLexicalExtension(LexicalExtension<TToken> extension,
            Func<TNode, Scope, LexicalExtension<TToken>, TNode> handler)
        {
            return (node, scope) => handler(node, scope, extension);
        }

        private class MatchInfo
        {
            public TokenSpan Span { get; set; }
            public ILexicalMatchResult<TToken, TNode, TModel> Match { get; set; }
        }
    }
}