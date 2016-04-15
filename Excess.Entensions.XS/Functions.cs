using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class Functions
    {
        public static void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical();
            var semantics = compiler.Semantics();

            lexical
                .Match() //lambda
                .Any(new [] { '(', '=', ','})
                .Token("function", "fn")
                .Enclosed('(', ')')
                .Token('{', "brace")
                .Then(compiler.Lexical().Transform()
                    .Remove("fn")
                    .Insert("=>", "brace"))
                .Match()
                .Token("function", "fn") //declarations
                .Identifier("id")
                .Enclosed('(', ')')
                .Token('{')
                .Then(lexical.Transform()
                    .Remove("fn")
                    .Then(ProcessMemberFunction, "id"));
            semantics
                .Error("CS0246", FunctionType);
        }

        private static SyntaxNode ProcessMemberFunction(SyntaxNode node, Scope scope)
        {
            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();

            if (node is MethodDeclarationSyntax)
            {
                var method = node as MethodDeclarationSyntax;
                if (method.ReturnType.IsMissing)
                {
                    document.Change(method, ReturnType);
                    return method.WithReturnType(RoslynCompiler.Void);
                }

                return node;
            }

            //handle functions declared inside code blocks
            var statement = node
                .AncestorsAndSelf()
                .OfType<StatementSyntax>()
                .FirstOrDefault();

            Debug.Assert(statement != null); //td: error, maybe?
            Debug.Assert(statement is ExpressionStatementSyntax);

            var invocation = (statement as ExpressionStatementSyntax)
                .Expression as InvocationExpressionSyntax;
            Debug.Assert(invocation != null);

            var function = invocation.Expression as IdentifierNameSyntax;
            Debug.Assert(function != null);

            var parent = statement.Parent as BlockSyntax;
            Debug.Assert(parent != null); //td: error, maybe?

            var body = RoslynCompiler.NextStatement(parent, statement) as BlockSyntax;
            if (body == null)
            {
                //td: error, function declaration must be followed by a block of code
                return node;
            }

            //We are not allowed to modify parents, so schedule the removal of the code
            //And its insertion in the final lambda variable
            document.Change(parent, RoslynCompiler.RemoveStatement(body));
            document.Change(statement, ProcessCodeFunction(function, body));
            return node;
        }

        private static SyntaxNode ReturnType(SyntaxNode node, SyntaxNode newNode, SemanticModel model, Scope scope)
        {
            var method = (MethodDeclarationSyntax) node;
            var type = RoslynCompiler.GetReturnType(method.Body, model);

            return (newNode as MethodDeclarationSyntax)
                .WithReturnType(type);
        }

        private static void FunctionType(SyntaxNode node, SemanticModel model, Scope scope)
        {
            TypeSyntax newType = null;
            if (node is GenericNameSyntax)
            {
                var generic = node as GenericNameSyntax;
                if (generic.Identifier.ToString() == "function")
                {
                    var arguments = new List<TypeSyntax>();
                    TypeSyntax returnType = null;

                    var first = true;
                    foreach (var arg in generic.TypeArgumentList.Arguments)
                    {
                        if (first)
                        {
                            first = false;
                            if (arg.ToString() != "void")
                                returnType = arg;
                        }
                        else
                            arguments.Add(arg);
                    }

                    if (returnType == null)
                        newType = generic
                            .WithIdentifier(CSharp.Identifier("Action"))
                            .WithTypeArgumentList(CSharp.TypeArgumentList(CSharp.SeparatedList(
                                arguments)));
                    else
                        newType = generic
                            .WithIdentifier(CSharp.Identifier("Func"))
                            .WithTypeArgumentList(CSharp.TypeArgumentList(CSharp.SeparatedList(
                                arguments
                                    .Union(new[] {returnType}))));
                }
            }
            else if (node.ToString() == "function")
            {
                newType = CSharp.ParseTypeName("Action");
            }

            if (newType != null)
            {
                var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                document.Change(node, RoslynCompiler.ReplaceNode(newType));
            }
        }

        private static Func<SyntaxNode, Scope, SyntaxNode> ProcessCodeFunction(IdentifierNameSyntax name, BlockSyntax body)
        {
            return (node, scope) =>
            {
                var localDeclaration = (LocalDeclarationStatementSyntax) CSharp.ParseStatement("var id = () => {}");
                var variable = localDeclaration.Declaration.Variables[0];
                var lambda = variable.Initializer.Value as ParenthesizedLambdaExpressionSyntax;
                Debug.Assert(lambda != null);

                return localDeclaration
                    .WithDeclaration(localDeclaration
                        .Declaration
                        .WithVariables(CSharp.SeparatedList(new[]
                        {
                            variable
                                .WithIdentifier(name.Identifier)
                                .WithInitializer(variable.Initializer
                                    .WithValue(lambda
                                        //.WithParameterList(invocation.ArgumentList) //td: extension arguments
                                        .WithBody(body)))
                        })));
            };
        }
    }
}