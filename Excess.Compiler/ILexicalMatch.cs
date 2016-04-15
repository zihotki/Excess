using System;
using System.Collections.Generic;

namespace Excess.Compiler
{
    public interface ILexicalMatch<TToken, TNode, TModel>
    {
        ILexicalMatch<TToken, TNode, TModel> Token(char token, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Token(string token, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Token(Func<TToken, bool> matcher, string named = null);

        ILexicalMatch<TToken, TNode, TModel> Any(params char[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Any(params string[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Any(char[] anyOf, string named = null, bool matchDocumentStart = false);
        ILexicalMatch<TToken, TNode, TModel> Any(string[] anyOf, string named = null, bool matchDocumentStart = false);
        ILexicalMatch<TToken, TNode, TModel> Any(Func<TToken, bool> anyOf, string named = null, bool matchDocumentStart = false);

        ILexicalMatch<TToken, TNode, TModel> Optional(params char[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Optional(params string[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Optional(char[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Optional(string[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Optional(Func<TToken, bool> anyOf, string named = null);

        ILexicalMatch<TToken, TNode, TModel> Enclosed(char open, char close, string start = null, string end = null, string contents = null);
        ILexicalMatch<TToken, TNode, TModel> Enclosed(string open, string close, string start = null, string end = null, string contents = null);

        ILexicalMatch<TToken, TNode, TModel> Enclosed(Func<TToken, bool> open,
            Func<TToken, bool> close,
            string start = null, string end = null, string contents = null);

        ILexicalMatch<TToken, TNode, TModel> Many(params char[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Many(params string[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> Many(char[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Many(string[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Many(Func<TToken, bool> tokens, string named = null);

        ILexicalMatch<TToken, TNode, TModel> ManyOrNone(params char[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> ManyOrNone(params string[] anyOf);
        ILexicalMatch<TToken, TNode, TModel> ManyOrNone(char[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> ManyOrNone(string[] anyOf, string named = null);
        ILexicalMatch<TToken, TNode, TModel> ManyOrNone(Func<TToken, bool> tokens, string named = null);

        ILexicalMatch<TToken, TNode, TModel> Identifier(string named = null, bool optional = false);

        ILexicalMatch<TToken, TNode, TModel> Until(char token, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Until(string token, string named = null);
        ILexicalMatch<TToken, TNode, TModel> Until(Func<TToken, bool> matcher, string named = null);

        ILexicalAnalysis<TToken, TNode, TModel> Then(Func<IEnumerable<TToken>, Scope, IEnumerable<TToken>> handler);
        ILexicalAnalysis<TToken, TNode, TModel> Then(Func<IEnumerable<TToken>, ILexicalMatchResult<TToken, TNode, TModel>, Scope, IEnumerable<TToken>> handler);
        ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, Scope, TNode> handler);
        ILexicalAnalysis<TToken, TNode, TModel> Then(Func<TNode, TNode, TModel, Scope, TNode> handler);
        ILexicalAnalysis<TToken, TNode, TModel> Then(ILexicalTransform<TToken, TNode, TModel> transform);

        ILexicalMatchResult<TToken, TNode, TModel> Match(IEnumerable<TToken> tokens, Scope scope, bool isDocumentStart);
    }
}