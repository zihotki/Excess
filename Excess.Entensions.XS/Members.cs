using Excess.Compiler;
using Excess.Compiler.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excess.Entensions.XS
{
    using CSharp = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using ExcessCompiler = ICompiler<SyntaxToken, SyntaxNode, SemanticModel>;

    public class Members
    {
        static public void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical();
            var syntax = compiler.Syntax();

            lexical
                //methods 
                .Match()
                    .Token("method", named: "keyword")
                    .Identifier(named: "id")
                    .Enclosed('(', ')')
                    .Token('{')
                    .Then(lexical.Transform()
                        .Remove("keyword")
                        .Then(Method, referenceToken: "id"))

                //properties
                .Match()
                    .Token("property")
                    .Identifier(named: "id")
                    .Token("=")
                    .Then(Property)

                .Match()
                    .Token("property", named: "keyword")
                    .Identifier(named: "id")
                    .Then(lexical.Transform()
                        .Remove("keyword")
                        .Then(Property, referenceToken: "id"));

            syntax
                //constructor
                .Match<MethodDeclarationSyntax>(method => method.ReturnType.IsMissing && method.Identifier.ToString() == "constructor")
                    .Then(Constructor);
        }

        private static PropertyDeclarationSyntax _property = SyntaxFactory.ParseCompilationUnit("__1 __2 {get; set;}")
            .DescendantNodes().OfType<PropertyDeclarationSyntax>().First();

        private static ExpressionStatementSyntax _assignment = (ExpressionStatementSyntax)SyntaxFactory.ParseStatement("__1 = __2;");

        private static SyntaxNode Property(SyntaxNode node, Scope scope)
        {
            var field = node.AncestorsAndSelf()
                .OfType<MemberDeclarationSyntax>()
                .FirstOrDefault()
                as FieldDeclarationSyntax;

            if (field == null)
            {
                //td: error, malformed property
                return node;
            }

            if (field.Declaration.Variables.Count != 1)
            {
                //td: error, malformed property
                return node;
            }

            var variable = field
                .Declaration
                .Variables[0];

            var initializer = variable.Initializer;
            var type = field.Declaration.Type;
            if (type == null || type.IsMissing || type.ToString() == "property") //untyped
            {
                if (initializer != null)
                    type = RoslynCompiler.ConstantType(initializer.Value);
            }

            if (type == null)
                type = RoslynCompiler.@dynamic;

            var property = _property
                .WithIdentifier(variable.Identifier)
                .WithType(type);

            if (!RoslynCompiler.HasVisibilityModifier(field.Modifiers))
                property = property.AddModifiers(CSharp.Token(SyntaxKind.PublicKeyword));

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();

            //schedule the field replacement
            //td: coud be done in this pass with the right info from lexical
            document.Change(field, RoslynCompiler.ReplaceNode(property));

            //must be initialized
            if (initializer != null)
            {
                var expr = (AssignmentExpressionSyntax)_assignment.Expression;
                document.Change(field.Parent, RoslynCompiler
                    .AddInitializers(_assignment.WithExpression(expr
                        .WithLeft(CSharp.IdentifierName(variable.Identifier))
                        .WithRight(initializer.Value))));
            }

            return node;
        }

        private static SyntaxNode Constructor(SyntaxNode node, Scope scope)
        {
            var method = (MethodDeclarationSyntax)node;

            string name = "__xs_constructor";

            ClassDeclarationSyntax parent = method.Parent as ClassDeclarationSyntax;
            if (parent != null)
            {
                name = parent.Identifier.ToString();
            }

            var modifiers = method.Modifiers.Any() ? method.Modifiers : RoslynCompiler.@public;
            return SyntaxFactory.ConstructorDeclaration(name).
                                    WithModifiers(modifiers).
                                    WithParameterList(method.ParameterList).
                                    WithBody(method.Body);
        }

        private static SyntaxNode Method(SyntaxNode node, Scope scope)
        {
            if (!(node is MethodDeclarationSyntax))
            {
                //td: error
                return node;
            }

            var method = node as MethodDeclarationSyntax;

            if (!RoslynCompiler.HasVisibilityModifier(method.Modifiers))
                method = method.AddModifiers(CSharp.Token(SyntaxKind.PublicKeyword));

            if (method.ReturnType.IsMissing)
            {
                var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                document.Change(method, FixReturnType);

                return method.WithReturnType(RoslynCompiler.@void);
            }

            return method;
        }

        private static SyntaxNode FixReturnType(SyntaxNode node, SyntaxNode newNode, SemanticModel model, Scope scope)
        {
            var method = (MethodDeclarationSyntax)node;
            var type = RoslynCompiler.GetReturnType(method.Body, model);

            return (newNode as MethodDeclarationSyntax)
                .WithReturnType(type);
        }
    }
}