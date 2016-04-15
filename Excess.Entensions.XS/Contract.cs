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

    public class Contract
    {
        private static readonly StatementSyntax ContractCheck = CSharp.ParseStatement(@"
            if (!(__condition)) 
                throw new InvalidOperationException(""Breach of contract!!"");");

        public static void Apply(ExcessCompiler compiler)
        {
            var syntax = compiler.Syntax();

            syntax
                .Extension("contract", ExtensionKind.Code, ProcessContract);
        }

        private static SyntaxNode ProcessContract(SyntaxNode node, Scope scope, SyntacticalExtension<SyntaxNode> extension)
        {
            if (extension.Kind == ExtensionKind.Code)
            {
                var block = extension.Body as BlockSyntax;
                Debug.Assert(block != null);

                var checks = new List<StatementSyntax>();
                foreach (var st in block.Statements)
                {
                    var stExpression = st as ExpressionStatementSyntax;
                    if (stExpression == null)
                    {
                        scope.AddError("contract01", "contracts only support boolean expressions", st);
                        continue;
                    }

                    var contractCheck = ContractCheck
                        .ReplaceNodes(ContractCheck
                            .DescendantNodes()
                            .OfType<ExpressionSyntax>()
                            .Where(expr => expr.ToString() == "__condition"),
                            (oldNode, newNode) =>
                                stExpression.Expression);

                    checks.Add(contractCheck);
                }

                return CSharp.Block(checks);
            }

            scope.AddError("contract02", "contract cannot return a value", node);
            return node;
        }
    }
}