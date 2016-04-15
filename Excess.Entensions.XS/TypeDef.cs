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

    internal class TypeDef
    {
        public static void Apply(ExcessCompiler compiler)
        {
            var lexical = compiler.Lexical();
            var semantics = compiler.Semantics();

            lexical
                .Match()
                .Token("typedef")
                .Identifier("id")
                .Token("=")
                .Until(';', "definition")
                .Then(TypedefAssignment)
                .Match()
                .Token("typedef", "keyword")
                .Until(';', "definition")
                .Then(compiler.Lexical().Transform()
                    .Remove("keyword")
                    .Then(Typedef));

            semantics
                .Error("CS0246", FixMissingType);
        }

        private static IEnumerable<SyntaxToken> TypedefAssignment(IEnumerable<SyntaxToken> tokens, Scope scope)
        {
            dynamic context = scope;

            SyntaxToken identifier = context.id;

            var found = false;
            foreach (var token in tokens)
            {
                if (found)
                {
                    if (token.Kind() == SyntaxKind.SemicolonToken)
                        break;

                    yield return token;
                }
                else
                    found = token.Kind() == SyntaxKind.EqualsToken;
            }

            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();

            yield return identifier.WithLeadingTrivia(CSharp.ParseLeadingTrivia(" "));
            yield return document.Change(CSharp.Token(SyntaxKind.SemicolonToken), Typedef);
        }

        private static SyntaxNode Typedef(SyntaxNode node, Scope scope)
        {
            var field = node
                .AncestorsAndSelf()
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault();

            if (field == null)
            {
                scope.AddError("xs01", "malformed typedef", node);
                //td: error, malformed typedef
                return node;
            }


            if (field.Declaration.Variables.Count != 1)
            {
                scope.AddError("xs01", "malformed typedef", node);
                return node;
            }

            var variable = field
                .Declaration
                .Variables[0];

            Debug.Assert(variable.Initializer == null || variable.Initializer.IsMissing);

            var type = RoslynCompiler.UnMark(field.Declaration.Type);
            var identifier = variable.Identifier;

            var parentScope = scope.CreateScope<SyntaxToken, SyntaxNode, SemanticModel>(field.Parent);
            Debug.Assert(parentScope != null);

            parentScope.set("__tdef" + identifier, type);

            //schedule deletion
            var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
            document.Change(field.Parent, RoslynCompiler.RemoveMember(field));

            //return intact
            return node;
        }

        private static void FixMissingType(SyntaxNode node, Scope scope)
        {
            var type = node
                .Ancestors()
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault();

            if (type != null)
            {
                var typeScope = scope.GetScope<SyntaxToken, SyntaxNode, SemanticModel>(type);
                if (typeScope != null)
                {
                    var realType = typeScope.get<SyntaxNode>("__tdef" + node);
                    if (realType != null)
                    {
                        realType = RoslynCompiler.Mark(realType); //make sure not to duplicate nodes

                        var document = scope.GetDocument<SyntaxToken, SyntaxNode, SemanticModel>();
                        document.Change(node, RoslynCompiler.ReplaceNode(realType));
                    }
                }
            }
        }
    }
}