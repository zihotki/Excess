﻿using Excess.Compiler.Core;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Excess.Compiler.Tests
{
    using Injector = ICompilerInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using DelegateInjector = DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using CompositeInjector = CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Dynamic;

    public class RuntimeHelper
    {
        private static Injector _main = new DelegateInjector(compiler =>
        {
            compiler.Lexical()
                .Normalize()
                .With(members: Normalize);
        });

        public static dynamic ExecuteTest(string text, Action<ICompiler<SyntaxToken, SyntaxNode, SemanticModel>> config)
        {
            var compilation = new Excess.Compiler.Roslyn.Compilation(null);
            var injector = new CompositeInjector<SyntaxToken, SyntaxNode, SemanticModel>(new[] {
                _main,
                new DelegateInjector<SyntaxToken, SyntaxNode, SemanticModel>(compiler => config(compiler))
            });

            compilation.addDocument("test", text, injector);

            Assembly assembly = compilation.build();
            if (assembly == null)
            {
                //debug
                StringBuilder errorLines = new StringBuilder();
                foreach (var error in compilation.errors())
                {
                    errorLines.AppendLine(error.ToString());
                }

                var errorString = errorLines.ToString();
                return null;
            }

            Type testtype = assembly.GetType("testclass");
            //var method = console.GetMethod("test", BindingFlags.Static);

            var result = new Dictionary<string, object>();
            testtype.InvokeMember("test", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[] { result });

            var xo = new ExpandoObject();
            var xod = xo as IDictionary<string, object>;
            foreach (var kp in result)
                xod.Add(kp.Key, kp.Value);

            return xo;
        }

        private static SyntaxNode Normalize(SyntaxNode root, IEnumerable<SyntaxNode> members, Scope scope)
        {
            var appClass = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(@class => @class.Identifier.ToString() == "testclass")
                .FirstOrDefault();

            if (appClass == null)
                appClass = CSharp.ClassDeclaration("testclass");

            return CSharp.CompilationUnit()
                .WithMembers(CSharp.List(new[] {(MemberDeclarationSyntax)
                appClass
                    .WithMembers(CSharp.List(
                     members.Select(
                         member => {
                             var method = member as MethodDeclarationSyntax;
                             if (method == null)
                                 return member;

                             return method.WithParameterList(
                                method
                                .ParameterList
                                .AddParameters(CSharp.Parameter(
                                    CSharp.ParseToken("result"))
                                    .WithType(CSharp.ParseTypeName("Dictionary<string, object>"))));})))}));
        }
    }
}
