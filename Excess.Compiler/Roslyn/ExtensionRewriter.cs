using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Compiler.Roslyn
{
    using CSharp = SyntaxFactory;

    public class ExtensionRewriter : CSharpSyntaxRewriter
    {
        private readonly Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>> _codeExtensions =
            new Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>>();

        private readonly Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>> _memberExtensions =
            new Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>>();

        private readonly Scope _scope;

        private readonly Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>> _typeExtensions =
            new Dictionary<string, Func<SyntaxNode, Scope, SyntacticalExtensionDto<SyntaxNode>, SyntaxNode>>();

        private Func<SyntaxNode, Scope, LookAheadResult> _lookahead;

        public ExtensionRewriter(IEnumerable<SyntacticalExtensionDto<SyntaxNode>> extensions, Scope scope)
        {
            _scope = scope;

            foreach (var extension in extensions)
            {
                switch (extension.Kind)
                {
                    case ExtensionKind.Code:
                        _codeExtensions[extension.Keyword] = extension.Handler;
                        break;
                    case ExtensionKind.Member:
                        _memberExtensions[extension.Keyword] = extension.Handler;
                        break;
                    case ExtensionKind.Type:
                        _typeExtensions[extension.Keyword] = extension.Handler;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (_lookahead != null)
            {
                var result = _lookahead(node, _scope);
                _lookahead = result.Continuation;
                return result.Result;
            }

            return base.Visit(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax method)
        {
            var extension = MethodExtension(method);
            if (extension != null)
            {
                switch (extension.Kind)
                {
                    case ExtensionKind.Member:
                    case ExtensionKind.Type:
                    {
                        return extension.Handler(method, new Scope(_scope), extension);
                    }
                    default:
                    {
                        //td: error, incorrect extension (i.e. a code extension being used inside a type)
                        return null;
                    }
                }
            }

            return base.VisitMethodDeclaration(method);
        }

        private SyntacticalExtensionDto<SyntaxNode> MethodExtension(MethodDeclarationSyntax method)
        {
            string extName = null;
            string extIdentifier = null;
            if (!method.ReturnType.IsMissing)
            {
                extName = method.ReturnType.ToString();
                extIdentifier = method.Identifier.ToString();
            }
            else
            {
                extName = method.Identifier.ToString();
            }

            if (method.Parent is CompilationUnitSyntax || method.Parent is NamespaceDeclarationSyntax)
            {
                if (_typeExtensions.ContainsKey(extName))
                {
                    return new SyntacticalExtensionDto<SyntaxNode>
                    {
                        Kind = ExtensionKind.Type,
                        Keyword = extName,
                        Identifier = extIdentifier,
                        Arguments = method.ParameterList,
                        Body = method.Body,
                        Handler = _typeExtensions[extName]
                    };
                }
            }


            if (!_memberExtensions.ContainsKey(extName))
            {
                return null;
            }

            return new SyntacticalExtensionDto<SyntaxNode>
            {
                Kind = ExtensionKind.Member,
                Keyword = extName,
                Identifier = extIdentifier,
                Arguments = method.ParameterList,
                Body = method.Body,
                Handler = _memberExtensions[extName]
            };
        }

        public override SyntaxNode VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            var extension = TypeExtension(node);
            if (extension != null)
            {
                if (extension.Kind == ExtensionKind.Type)
                {
                    _lookahead = MatchTypeExtension(node, extension);
                    return null; //remove the incomplete member
                }
                //td: error, incorrect extension (i.e. a code extension being used inside a type)
                return null;
            }

            return node; //error, stop processing
        }

        private Func<SyntaxNode, Scope, LookAheadResult> MatchTypeExtension(IncompleteMemberSyntax incomplete, SyntacticalExtensionDto<SyntaxNode> extension)
        {
            return (node, scope) =>
            {
                var resulSyntaxNode = node;
                if (node is ClassDeclarationSyntax)
                {
                    var clazz = (ClassDeclarationSyntax) node;
                    clazz = clazz
                        .WithAttributeLists(incomplete.AttributeLists)
                        .WithModifiers(incomplete.Modifiers);

                    resulSyntaxNode = extension.Handler(node, scope, extension);
                }

                //td: error?, expecting class
                return new LookAheadResult
                {
                    Matched = resulSyntaxNode != null,
                    Result = resulSyntaxNode
                };
            };
        }

        private SyntacticalExtensionDto<SyntaxNode> TypeExtension(IncompleteMemberSyntax node)
        {
            throw new NotImplementedException();
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var extension = null as SyntacticalExtensionDto<SyntaxNode>;
            var expr = node.Expression;
            InvocationExpressionSyntax call = null;

            if (expr is InvocationExpressionSyntax)
            {
                call = expr as InvocationExpressionSyntax;
            }
            else if (expr is AssignmentExpressionSyntax)
            {
                var assignment = expr as AssignmentExpressionSyntax;
                call = assignment.Right as InvocationExpressionSyntax;
            }

            if (call != null)
            {
                extension = CodeExtension(call);
            }

            if (extension != null)
            {
                if (extension.Kind != ExtensionKind.Code)
                {
                    //td: error, incorrect extension (i.e. a code extension being used inside a type)
                    return node;
                }

                _lookahead = CheckCodeExtension(node, extension);
                return null;
            }

            return node;
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declaration.Variables.Count == 1)
            {
                var variable = node
                    .Declaration
                    .Variables[0];

                var call = variable.Initializer?.Value as InvocationExpressionSyntax;
                if (call != null)
                {
                    var extension = CodeExtension(call);
                    if (extension != null)
                    {
                        if (extension.Kind != ExtensionKind.Code)
                        {
                            //td: error, incorrect extension (i.e. a code extension being used inside a type)
                            return node;
                        }

                        _lookahead = CheckCodeExtension(node, extension);
                        return null;
                    }
                }
            }

            return base.VisitLocalDeclarationStatement(node);
        }

        private SyntacticalExtensionDto<SyntaxNode> CodeExtension(InvocationExpressionSyntax call)
        {
            if (!(call.Expression is SimpleNameSyntax))
            {
                return null; //extensions are simple identifiers
            }

            var extName = call.Expression.ToString();
            if (!_codeExtensions.ContainsKey(extName))
            {
                return null; //not an extension
            }

            return new SyntacticalExtensionDto<SyntaxNode>
            {
                Kind = ExtensionKind.Code,
                Keyword = extName,
                Identifier = null,
                Arguments = call.ArgumentList,
                Body = null,
                Handler = _codeExtensions[extName]
            };
        }

        //rewriters
        private Func<SyntaxNode, Scope, LookAheadResult> CheckCodeExtension(SyntaxNode original, SyntacticalExtensionDto<SyntaxNode> extension)
        {
            return (node, scope) =>
            {
                var code = node as BlockSyntax;
                if (code == null)
                {
                    return new LookAheadResult {Matched = false};
                }

                _lookahead = null;
                extension.Body = base.Visit(code);

                SyntaxNode resulSyntaxNode = null;

                if (original is LocalDeclarationStatementSyntax)
                {
                    extension.Kind = ExtensionKind.Expression;
                    resulSyntaxNode = extension.Handler(node, scope, extension);
                    if (!(resulSyntaxNode is ExpressionSyntax))
                    {
                        //td: error, expecting expression
                        return new LookAheadResult {Matched = false};
                    }

                    var localDecl = (LocalDeclarationStatementSyntax) original;
                    resulSyntaxNode = localDecl
                        .WithDeclaration(localDecl.Declaration
                            .WithVariables(CSharp.SeparatedList(new[]
                            {
                                localDecl.Declaration.Variables[0]
                                    .WithInitializer(localDecl.Declaration.Variables[0].Initializer
                                        .WithValue((ExpressionSyntax) resulSyntaxNode))
                            })))
                        .WithSemicolonToken(CSharp.ParseToken(";"));
                }
                else if (original is ExpressionStatementSyntax)
                {
                    var exprStatement = (ExpressionStatementSyntax) original;
                    var assignment = exprStatement.Expression as AssignmentExpressionSyntax;
                    if (assignment != null)
                    {
                        extension.Kind = ExtensionKind.Expression;
                        resulSyntaxNode = extension.Handler(node, scope, extension);
                        if (!(resulSyntaxNode is ExpressionSyntax))
                        {
                            //td: error, expecting expression
                            return new LookAheadResult {Matched = false};
                        }

                        resulSyntaxNode = exprStatement
                            .WithExpression(assignment
                                .WithRight((ExpressionSyntax) resulSyntaxNode))
                            .WithSemicolonToken(CSharp.ParseToken(";"));
                    }
                    else
                    {
                        resulSyntaxNode = extension.Handler(node, scope, extension);
                        if (resulSyntaxNode != null)
                        {
                            resulSyntaxNode = RoslynCompiler.UpdateExcessId(resulSyntaxNode, node);
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                return new LookAheadResult
                {
                    Matched = true,
                    Result = resulSyntaxNode
                };
            };
        }

        private class LookAheadResult
        {
            public bool Matched { get; set; }
            public SyntaxNode Result { get; set; }
            public Func<SyntaxNode, Scope, LookAheadResult> Continuation { get; set; }
        }
    }
}