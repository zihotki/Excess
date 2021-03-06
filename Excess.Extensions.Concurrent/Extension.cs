﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace Excess.Extensions.Concurrent
{
    using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using Roslyn = RoslynCompiler;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;
    using Model;

    public class Extension
    {
        public static void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical();

            lexical
                .match()
                    .token("concurrent", named: "keyword")
                    .token("class", named: "ref")
                    .then(lexical.transform()
                        .remove("keyword")
                        .then(Compile))
                .match()
                    .token("concurrent", named: "keyword")
                    .token("object", named: "ref")
                    .then(lexical.transform()
                        .remove("keyword")
                        .replace("ref", "class")
                        .then(CompileObject));
        }

        private static SyntaxNode Compile(SyntaxNode node, Scope scope)
        {
            Debug.Assert(node is ClassDeclarationSyntax);
            var @class = (node as ClassDeclarationSyntax)
                .AddBaseListTypes(CSharp.SimpleBaseType(
                    CSharp.ParseTypeName("ConcurrentObject")));

            var className = @class.Identifier.ToString();

            var ctx = new Class(className, scope);
            scope.set<Class>(ctx);

            foreach (var member in @class.Members)
            {
                if (member is PropertyDeclarationSyntax)
                    compileProperty(member as PropertyDeclarationSyntax, ctx, scope);
                else if (member is MethodDeclarationSyntax)
                {
                    if (compileMethod(member as MethodDeclarationSyntax, ctx, scope))
                        ctx.RemoveMember(member);
                }
            }

            @class = ctx.Update(@class);

            var document = scope.GetDocument();
            return document.change(@class, Link(ctx), null);
        }

        private static Func<SyntaxNode, SyntaxNode, SemanticModel, Scope, SyntaxNode> Link(Class ctx)
        {
            return (oldNode, newNode, model, scope) =>
            {
                Debug.Assert(newNode is ClassDeclarationSyntax);
                var @class = new ClassLinker(ctx, model)
                    .Visit(newNode);

                Debug.Assert(@class != null);
                Debug.Assert(@class is ClassDeclarationSyntax);
                return ctx.Update(@class as ClassDeclarationSyntax);
            };
        }

        private static void compileProperty(PropertyDeclarationSyntax property, Class ctx, Scope scope)
        {
            if (!Roslyn.IsVisible(property))
                return;

            var @get = null as AccessorDeclarationSyntax;
            var @set = null as AccessorDeclarationSyntax;
            foreach (var accesor in property.AccessorList.Accessors)
            {
                switch (accesor.Keyword.Kind())
                {
                    case SyntaxKind.GetKeyword:
                        @get = accesor;
                        break;
                    case SyntaxKind.SetKeyword:
                        @set = accesor;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            bool hasCustomGet = @get != null && @get.Body != null && @get.Body.Statements.Count > 0;
            if (hasCustomGet && @get.Body.Statements.Count == 1)
                hasCustomGet = !(@get.Body.Statements[0] is ReturnStatementSyntax);

            bool hasCustomSet = @set != null && @set.Body != null && @set.Body.Statements.Count > 0;
            if (hasCustomSet && @set.Body.Statements.Count == 1)
                hasCustomSet = !(@set.Body.Statements[0] is ExpressionStatementSyntax)
                            || (@set.Body.Statements[0] as ExpressionStatementSyntax)
                                    .Expression.Kind() != SyntaxKind.SimpleAssignmentExpression;

            if (hasCustomGet || hasCustomSet)
            {
                scope.AddError("concurrent00", "invalid concurrent property, custom accessors are not allowed", property);
            }
        }

        private static bool compileMethod(MethodDeclarationSyntax methodDeclaration, Class ctx, Scope scope)
        {
            var method = methodDeclaration;
            var name = method.Identifier.ToString();
            var isMain = name == "main";

            var isProtected = method
                .Modifiers
                .Where(m => m.Kind() == SyntaxKind.ProtectedKeyword)
                .Any();

            var isVisible = isProtected || Roslyn.IsVisible(method);

            var hasReturnType = method.ReturnType.ToString() != "void";
            var returnType = hasReturnType 
                ? method.ReturnType
                : Roslyn.boolean;

            var isEmptySignal = method.Body == null 
                             || method.Body.IsMissing;
            if (isEmptySignal)
            {
                if (method.ParameterList.Parameters.Count > 0)
                    scope.AddError("concurrent03", "empty signals cannot contain parameters", method);

                if (method.ReturnType.ToString() != "void")
                    scope.AddError("concurrent04", "empty signals cannot return values", method);

                method = method
                    .WithSemicolonToken(CSharp.MissingToken(SyntaxKind.SemicolonToken))
                    .WithBody(CSharp.Block());
            }

            var cc = parseConcurrentBlock(ctx, method.Body, scope);
            if (cc != null)
                method = method.WithBody(cc);

            //remove attributes, until needed
            method = method.WithAttributeLists(CSharp.List<AttributeListSyntax>());

            if (isMain)
            {
                if (ctx.HasMain)
                {
                    scope.AddError("concurrent06", "multiple main methods", method);
                    return false;
                }

                ctx.HasMain = true;

                var statements = method.Body.Statements;
                var isContinued = (cc == null) && checkContinued(statements);
                if (isContinued)
                {
                    method = method
                        .WithBody(CSharp.Block(statements
                            .Take(statements.Count - 1)));
                }

                var mainMethod = concurrentMethod(ctx, method);

                //hook up our start method
                int currentIndex = 0;
                ctx.AddMember(Templates
                    .StartObject
                    .Get<MethodDeclarationSyntax>(Templates
                        .ConcurrentMain
                        .Get<InvocationExpressionSyntax>()
                        .WithArgumentList(CSharp.ArgumentList(CSharp.SeparatedList(
                            mainMethod
                            .ParameterList
                            .Parameters
                            .Where(param => param.Identifier.ToString() != "__success"
                                         && param.Identifier.ToString() != "__failure")
                            .Select(param => CSharp.Argument(Templates
                                .StartObjectArgument
                                .Get<ExpressionSyntax>(
                                    param.Type,
                                    currentIndex++)))
                             .Union(new[] {
                                 CSharp.Argument(Roslyn.@null),
                                 CSharp.Argument(Roslyn.@null),
                             }))))));

                return true;
            }

            if (isVisible)
            {
                if (isEmptySignal)
                    ctx.AddMember(Templates
                        .EmptySignalMethod
                        .Get<MethodDeclarationSyntax>(
                            "__concurrent" + name, 
                            Roslyn.Quoted(name),
                            isProtected ? Roslyn.@true : Roslyn.@false));
                else
                    concurrentMethod(ctx, method);
            }
            else if (cc != null)
                concurrentMethod(ctx, method);
            else if (isEmptySignal)
                ctx.AddMember(Templates
                    .EmptySignalMethod
                    .Get<MethodDeclarationSyntax>(
                        "__concurrent" + name, 
                        Roslyn.Quoted(name),
                        isProtected? Roslyn.@true : Roslyn.@false));
            else
                return false;

            var signal = ctx.AddSignal(name, returnType, isVisible);
            if (isVisible)
                createPublicSignals(ctx, method, signal);

            return true;
        }

        private static BlockSyntax parseConcurrentBlock(Class ctx, BlockSyntax body, Scope scope)
        {
            var rewriter = new BlockRewriter(ctx, scope);
            var result = (BlockSyntax)rewriter.Visit(body);

            if (rewriter.HasConcurrent)
                return result;

            return null;
        }

        private static MethodDeclarationSyntax concurrentMethod(Class ctx, MethodDeclarationSyntax method, bool forever = false)
        {
            var name = method.Identifier.ToString();

            var body = method.Body;
            var returnStatements = body
                .DescendantNodes()
                .OfType<ReturnStatementSyntax>();

            var lastReturn = body.Statements.LastOrDefault() as ReturnStatementSyntax;

            if (returnStatements.Any())
                body = body
                    .ReplaceNodes(returnStatements, 
                    (on, nn) => Templates
                        .ExpressionReturn
                        .Get<StatementSyntax>(nn.Expression == null || nn.Expression.IsMissing
                            ? Roslyn.@null
                            : nn.Expression,
                            Roslyn.Quoted(method.Identifier.ToString())));

            if (forever)
            {
                body = CSharp.Block(
                    CSharp.ForStatement(body));
            }
            else if (lastReturn == null)
                body = body.AddStatements(Templates
                    .ExpressionReturn
                    .Get<StatementSyntax>(
                        Roslyn.@null,
                        Roslyn.Quoted(method.Identifier.ToString())));

            var result = Templates
                .ConcurrentMethod
                .Get<MethodDeclarationSyntax>("__concurrent" + name);
            result = result
                .WithParameterList(method
                    .ParameterList
                    .AddParameters(result
                        .ParameterList
                        .Parameters
                        .ToArray()))
                .WithBody(body);

            ctx.AddMember(result);
            return result;
        }

        private static bool checkContinued(IEnumerable<StatementSyntax> statements)
        {
            var statement = statements.LastOrDefault();
            return (statement != null && statement is ContinueStatementSyntax);
        }

        private static void createPublicSignals(Class ctx, MethodDeclarationSyntax method, Signal signal)
        {
            var returnType = method.ReturnType.ToString() != "void"
                ? method.ReturnType
                : Roslyn.@object;

            var internalMethod = method.Identifier.ToString();
            if (!internalMethod.StartsWith("__concurrent"))
                internalMethod = "__concurrent" + internalMethod;

            var internalCall = Templates
                .InternalCall
                .Get<ExpressionSyntax>(internalMethod);

            internalCall = internalCall
                .ReplaceNodes(internalCall
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(i => i.ArgumentList.Arguments.Count == 2),
                (on, nn) => nn
                    .WithArgumentList(CSharp.ArgumentList(CSharp.SeparatedList(
                        method.ParameterList.Parameters
                            .Select(param => CSharp.Argument(CSharp.IdentifierName(
                                param.Identifier)))
                        .Union(on.ArgumentList.Arguments)))));

            ctx.AddMember(AddParameters(
                method.ParameterList,
                Templates.TaskPublicMethod
                .Get<MethodDeclarationSyntax>(
                    method.Identifier.ToString(),
                    returnType,
                    internalCall)));

            ctx.AddMember(AddParameters(
                method.ParameterList,
                Templates.TaskCallbackMethod
                .Get<MethodDeclarationSyntax>(
                    method.Identifier.ToString(),
                    internalCall)));
        }

        private static MemberDeclarationSyntax AddParameters(ParameterListSyntax parameterList, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            return methodDeclarationSyntax
            .WithParameterList(methodDeclarationSyntax.ParameterList
                .WithParameters(CSharp.SeparatedList(
                    parameterList.Parameters
                    .Union(methodDeclarationSyntax
                        .ParameterList.Parameters))));
        }

        private static SyntaxNode CompileObject(SyntaxNode node, Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}
