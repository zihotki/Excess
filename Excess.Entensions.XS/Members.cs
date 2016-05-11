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

    public class Members
    {
        private static readonly PropertyDeclarationSyntax _property = CSharp.ParseCompilationUnit("__1 __2 {get; set;}")
            .DescendantNodes().OfType<PropertyDeclarationSyntax>().First();

        private static readonly ExpressionStatementSyntax _assignment = (ExpressionStatementSyntax)CSharp.ParseStatement("__1 = __2;");

        public static void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical;
            var syntax = compiler.Syntax;

            lexical
                //methods 
                .Match()
                .Token("method", "keyword")
                .Identifier("id")
                .Enclosed('(', ')')
                .Token('{')
                .Then(lexical.Transform()
                    .Remove("keyword")
                    .Then(Method, "id"))

                //properties
                .Match()
                .Token("property")
                .Identifier("id")
                .Token("=")
                .Then(Property)
                .Match()
                .Token("property", "keyword")
                .Identifier("id")
                .Then(lexical.Transform()
                    .Remove("keyword")
                    .Then(Property, "id"));

            syntax
                //constructor
                .Match<MethodDeclarationSyntax>(method => method.ReturnType.IsMissing && method.Identifier.ToString() == "constructor")
                .Then(Constructor);
        }

        private static SyntaxNode Property(SyntaxNode node, Scope scope)
        {
            var field = node.AncestorsAndSelf()
                .OfType<MemberDeclarationSyntax>()
                .FirstOrDefault()
                as FieldDeclarationSyntax;

            if (field == null)
            {
                //TODO: error, malformed property
                return node;
            }

            if (field.Declaration.Variables.Count != 1)
            {
                //TODO: error, malformed property
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
                type = RoslynCompiler.Dynamic;

            var property = _property
                .WithIdentifier(variable.Identifier)
                .WithType(type);

            if (!RoslynCompiler.HasVisibilityModifier(field.Modifiers))
                property = property.AddModifiers(CSharp.Token(SyntaxKind.PublicKeyword));

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();

            //schedule the field replacement
            //TODO: coud be done in this pass with the right info from lexical
            document.Change(field, RoslynCompiler.ReplaceNode(property));

            //must be initialized
            if (initializer != null)
            {
                var expr = (AssignmentExpressionSyntax) _assignment.Expression;
                document.Change(field.Parent, RoslynCompiler
                    .AddInitializers(_assignment.WithExpression(expr
                        .WithLeft(CSharp.IdentifierName(variable.Identifier))
                        .WithRight(initializer.Value))));
            }

            return node;
        }

        private static SyntaxNode Constructor(SyntaxNode node, Scope scope)
        {
            var method = (MethodDeclarationSyntax) node;

            var name = "__xs_constructor";

            var parent = method.Parent as ClassDeclarationSyntax;
            if (parent != null)
            {
                name = parent.Identifier.ToString();
            }

            var modifiers = method.Modifiers.Any()
                ? method.Modifiers
                : RoslynCompiler.Public;
            return CSharp.ConstructorDeclaration(name).
                WithModifiers(modifiers).
                WithParameterList(method.ParameterList).
                WithBody(method.Body);
        }

        private static SyntaxNode Method(SyntaxNode node, Scope scope)
        {
            if (!(node is MethodDeclarationSyntax))
            {
                //TODO: error
                return node;
            }

            var method = node as MethodDeclarationSyntax;

            if (!RoslynCompiler.HasVisibilityModifier(method.Modifiers))
                method = method.AddModifiers(CSharp.Token(SyntaxKind.PublicKeyword));

            if (method.ReturnType.IsMissing)
            {
                var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                document.Change(method, FixReturnType);

                return method.WithReturnType(RoslynCompiler.Void);
            }

            return method;
        }

        private static SyntaxNode FixReturnType(SyntaxNode node, SyntaxNode newNode, SemanticModel model, Scope scope)
        {
            var method = (MethodDeclarationSyntax) node;
            var type = RoslynCompiler.GetReturnType(method.Body, model);

            return (newNode as MethodDeclarationSyntax)
                .WithReturnType(type);
        }
    }
}