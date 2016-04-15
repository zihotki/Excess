using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RazorEngine;
using RazorEngine.Templating;

namespace Excess.Compiler.Razor
{
    using CSharp = SyntaxFactory;

    internal enum TemplateKind
    {
        Expression,
        Statement,
        Code,
        Text
    }

    public class RazorTemplate
    {
        private readonly string _key;

        private readonly TemplateKind _kind;

        internal RazorTemplate(TemplateKind kind, string key)
        {
            _kind = kind;
            _key = key;
        }

        public static RazorTemplate ParseExpression(string text)
        {
            return new RazorTemplate(TemplateKind.Expression, ParseRazor(text));
        }

        public static RazorTemplate ParseExpression<T>(string text)
        {
            return new RazorTemplate(TemplateKind.Expression, ParseRazor(text, typeof(T)));
        }

        public static RazorTemplate ParseStatement(string text)
        {
            return new RazorTemplate(TemplateKind.Statement, ParseRazor(text));
        }

        public static RazorTemplate ParseStatement<T>(string text)
        {
            return new RazorTemplate(TemplateKind.Statement, ParseRazor(text, typeof(T)));
        }

        public static RazorTemplate ParseStatements(string text)
        {
            return new RazorTemplate(TemplateKind.Statement, ParseRazor("{" + text + "}"));
        }

        public static RazorTemplate ParseStatements<T>(string text)
        {
            return new RazorTemplate(TemplateKind.Statement, ParseRazor("{" + text + "}", typeof(T)));
        }

        public static RazorTemplate Parse(string text)
        {
            return new RazorTemplate(TemplateKind.Code, ParseRazor(text));
        }

        public static RazorTemplate Parse<T>(string text)
        {
            return new RazorTemplate(TemplateKind.Code, ParseRazor(text, typeof(T)));
        }

        public static RazorTemplate ParseText(string text)
        {
            return new RazorTemplate(TemplateKind.Text, ParseRazor(text));
        }

        public static RazorTemplate ParseText<T>(string text)
        {
            return new RazorTemplate(TemplateKind.Text, ParseRazor(text, typeof(T)));
        }

        private static string ParseRazor(string text, Type modelType = null)
        {
            var result = Guid.NewGuid().ToString();
            Engine.Razor.Compile(text, result, modelType);
            return result;
        }

        public string Render(object model)
        {
            return Engine.Razor.Run(_key, null, model);
        }

        public SyntaxNode Get(object model)
        {
            if (_kind == TemplateKind.Text)
                throw new InvalidOperationException("asking syntax nodes from text");

            var text = Render(model);
            switch (_kind)
            {
                case TemplateKind.Code:
                    return CSharp.ParseCompilationUnit(text);
                case TemplateKind.Expression:
                    return CSharp.ParseExpression(text);
                case TemplateKind.Statement:
                    return CSharp.ParseStatement(text);
            }

            throw new NotImplementedException();
        }

        public T Get<T>(object model) where T : SyntaxNode
        {
            var result = Get(model);
            return result
                .DescendantNodesAndSelf()
                .OfType<T>()
                .First();
        }
    }
}