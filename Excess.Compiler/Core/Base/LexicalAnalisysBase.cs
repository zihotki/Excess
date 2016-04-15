using System;
using System.Collections.Generic;
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

                if (matchNone || (matchDocumentStart && match.IsDocumentStart()))
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

                if (matchNone || (matchDocumentStart && match.IsDocumentStart()))
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

            private readonly List<LexicalMatchItemDto> _items = new List<LexicalMatchItemDto>();
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

            private TokenSpanDto Consume(int count)
            {
                var result = new TokenSpanDto(_current, count);
                _current += count;
                _tokens = _tokens.Skip(count);
                return result;
            }

            public void AddResult(int length, string identifier, bool literal = false)
            {
                _items.Add(new LexicalMatchItemDto(Consume(length), identifier, literal));
            }

            public bool IsDocumentStart()
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
}