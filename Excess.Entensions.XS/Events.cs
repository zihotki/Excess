using System;
using System.Collections.Generic;
using System.Linq;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Entensions.XS
{
    using CSharp = SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;

    internal class Events
    {
        private static readonly StatementSyntax _EventInitializer = CSharp.ParseStatement("__1 += __2;");

        public static void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical();
            var syntax = compiler.Syntax();

            lexical
                .Match()
                .Token("event", "ev")
                .Identifier("id")
                .Enclosed('(', ')', contents: "args")
                .Then(LexicalEventDeclaration);

            syntax
                .Match<MethodDeclarationSyntax>(method => method.ReturnType.ToString() == "on")
                .Then(EventHandler);
        }

        private static SyntaxNode EventHandler(SyntaxNode node, Scope scope)
        {
            var method = (MethodDeclarationSyntax) node;
            if (method.Modifiers.Any())
            {
                //td: error, no modifiers allowed 
                return node;
            }

            var args = method.ParameterList.ReplaceNodes(method.ParameterList.Parameters, (oldParam, newParam) =>
            {
                if (oldParam.Identifier.IsMissing)
                    return newParam
                        .WithType(RoslynCompiler.@void)
                        .WithIdentifier(CSharp.Identifier(newParam.Type.ToString()));

                return newParam;
            });

            var result = method
                .WithReturnType(RoslynCompiler.@void)
                .WithParameterList(args)
                .WithModifiers(RoslynCompiler.@private);

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
            return document.Change(result, SemanticEventHandler);
        }

        private static SyntaxNode SemanticEventHandler(SyntaxNode oldNode, SyntaxNode node, SemanticModel model, Scope scope)
        {
            var mthd = (MethodDeclarationSyntax) node;
            var methdArgs = mthd.ParameterList;
            var methodName = "on_" + mthd.Identifier;

            var self = model.GetDeclaredSymbol(oldNode);
            var type = (ITypeSymbol) self.ContainingSymbol;

            var evName = mthd.Identifier.ToString();
            var typeName = type.Name;
            var found = false;
            while (type != null && !found)
            {
                foreach (var ev in type.GetMembers().OfType<IEventSymbol>())
                {
                    if (ev.Name.Equals(evName))
                    {
                        //arguments
                        foreach (var syntax in ev.Type.DeclaringSyntaxReferences)
                        {
                            var refNode = (DelegateDeclarationSyntax) syntax.GetSyntax();

                            var pCount = methdArgs.Parameters.Count;
                            var idx = 0;
                            var match = true;
                            methdArgs = refNode.ParameterList.ReplaceNodes(refNode.ParameterList.Parameters, (oldArg, newArg) =>
                            {
                                if (match)
                                {
                                    if (idx >= pCount && match)
                                        return newArg;

                                    var arg = methdArgs.Parameters[idx++];
                                    var argName = arg.Identifier.ToString();
                                    if (argName == oldArg.Identifier.ToString())
                                    {
                                        //coincident parameters, fix missing type or return same
                                        if (arg.Identifier.IsMissing || arg.Type.ToString() == "void")
                                            return newArg.WithIdentifier(arg.Identifier);

                                        return arg;
                                    }
                                    match = false;
                                    if (!refNode.ParameterList.Parameters.Any(p => p.Identifier.ToString().Equals(arg.Identifier.ToString())))
                                    {
                                        //name change?
                                        if (oldArg.Identifier.IsMissing)
                                            return newArg.WithIdentifier(CSharp.Identifier(arg.Type.ToString()));

                                        return arg;
                                    }
                                }

                                return newArg;
                            });
                        }

                        //event initialization
                        var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                        document.Change(mthd.Parent, RoslynCompiler.AddInitializers(EventInitializer(ev.Name, methodName)));
                        found = true;
                        break;
                    }
                }

                type = type.BaseType;
            }

            if (!found)
            {
                //td: error, no such event
            }

            return mthd.WithIdentifier(CSharp.Identifier(methodName)).
                WithParameterList(methdArgs);
        }

        public static StatementSyntax EventInitializer(string addMethod, string implementor)
        {
            return _EventInitializer.ReplaceNodes(_EventInitializer.DescendantNodes().OfType<IdentifierNameSyntax>(), (oldNode, newNode) =>
            {
                var id = oldNode.Identifier.ToString() == "__1"
                    ? addMethod
                    : implementor;
                return newNode.WithIdentifier(CSharp.Identifier(id));
            });
        }

        private static IEnumerable<SyntaxToken> LexicalEventDeclaration(IEnumerable<SyntaxToken> tokens, Scope scope)
        {
            dynamic context = scope;

            SyntaxToken keyword = context.ev;
            SyntaxToken identifier = context.id;
            IEnumerable<SyntaxToken> args = context.args;

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();

            yield return document.Change(keyword, EventDeclaration(args));
            yield return CSharp.Identifier(" useless ");
            yield return identifier;
        }

        private static Func<SyntaxNode, Scope, SyntaxNode> EventDeclaration(IEnumerable<SyntaxToken> args)
        {
            return (node, scope) =>
            {
                var @event = (EventFieldDeclarationSyntax) node;
                var @params = CSharp.ParseParameterList(RoslynCompiler.TokensToString(args));

                var variable = @event
                    .Declaration
                    .Variables[0];

                var delegateName = variable.Identifier.ToString() + "_delegate"; //td: unique ids
                var delegateDecl = CSharp.DelegateDeclaration(RoslynCompiler.@void, delegateName)
                    .WithParameterList(@params)
                    .WithModifiers(@event.Modifiers);

                //add the delegate
                var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                document.Change(@event.Parent, RoslynCompiler.AddMember(delegateDecl));

                return @event
                    .WithDeclaration(@event.Declaration
                        .WithType(CSharp.ParseTypeName(delegateName)));
            };
        }
    }
}