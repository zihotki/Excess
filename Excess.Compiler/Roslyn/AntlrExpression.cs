using System;
using System.Collections.Generic;
using System.Diagnostics;
using Antlr4.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Excess.Compiler.Roslyn
{
    using CSharp = SyntaxFactory;

    public static class AntlrExpression
    {
        private static readonly Dictionary<string, Func<ParserRuleContext, ExpressionSyntax>> Handlers =
            new Dictionary<string, Func<ParserRuleContext, ExpressionSyntax>>();

        static AntlrExpression()
        {
            Handlers["ExpressionContext"] = Expression;
            Handlers["AssignmentExpressionContext"] = Assignment;
            Handlers["UnaryExpressionContext"] = UnaryExpression;
            Handlers["LogicalAndExpressionContext"] = BinaryExpression;
            Handlers["AdditiveExpressionContext"] = BinaryExpression;
            Handlers["ConditionalExpressionContext"] = BinaryExpression;
            Handlers["LogicalOrExpressionContext"] = BinaryExpression;
            Handlers["InclusiveOrExpressionContext"] = BinaryExpression;
            Handlers["ExclusiveOrExpressionContext"] = BinaryExpression;
            Handlers["AndExpressionContext"] = BinaryExpression;
            Handlers["EqualityExpressionContext"] = BinaryExpression;
            Handlers["ConstantContext"] = Constant;
            Handlers["CastExpressionContext"] = Cast;
            Handlers["RelationalExpressionContext"] = BinaryExpression;
            Handlers["ShiftExpressionContext"] = BinaryExpression;
            Handlers["MultiplicativeExpressionContext"] = BinaryExpression;
            Handlers["MemberAccessContext"] = MemberAccess;
            Handlers["ParenthesizedContext"] = Parenthesized;
            Handlers["MemberPointerAccessContext"] = MemberAccess;
            Handlers["SimpleExpressionContext"] = Expression;
            Handlers["IndexContext"] = Index;
            Handlers["CallContext"] = Call;
            Handlers["PostfixIncrementContext"] = PostFix;
            Handlers["PostfixDecrementContext"] = PostFix;
            Handlers["IdentifierContext"] = Identifer;
            Handlers["StringLiteralContext"] = StringLiteral;
            Handlers["ArgumentExpressionListContext"] = Hidden;
            Handlers["UnaryOperatorContext"] = Hidden;
            Handlers["AssignmentOperatorContext"] = Hidden;
            Handlers["ConstantExpressionContext"] = Hidden;
        }

        public static SyntaxNode Parse(ParserRuleContext node, Func<ParserRuleContext, Scope, SyntaxNode> continuation, Scope scope)
        {
            return VisitNode(node); //TODO: scope needed?
        }

        private static ExpressionSyntax StringLiteral(ParserRuleContext arg)
        {
            return CSharp.ParseExpression(arg.GetText());
        }

        private static ExpressionSyntax Hidden(ParserRuleContext arg)
        {
            throw new InvalidOperationException("This node should not be processed directly");
        }

        private static ExpressionSyntax Identifer(ParserRuleContext arg)
        {
            return CSharp.IdentifierName(arg.GetText());
        }

        private static ExpressionSyntax Expression(ParserRuleContext node)
        {
            Debug.Assert(node.ChildCount == 1);
            return VisitNode(node.GetRuleContext<ParserRuleContext>(0));
        }

        private static ExpressionSyntax Assignment(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 3);
            var left = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            var right = VisitNode(node.GetRuleContext<ParserRuleContext>(2));

            SyntaxKind kind;
            var op = GetBinaryOperator(node.children[1].GetText(), out kind);

            return CSharp.AssignmentExpression(kind, left, op, right);
        }

        private static SyntaxToken GetBinaryOperator(string value, out SyntaxKind kind)
        {
            var result = CSharp.ParseToken(value);
            kind = result.Kind();
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    kind = SyntaxKind.AddExpression;
                    break;
                case SyntaxKind.MinusToken:
                    kind = SyntaxKind.SubtractExpression;
                    break;
                case SyntaxKind.AsteriskToken:
                    kind = SyntaxKind.MultiplyExpression;
                    break;
                case SyntaxKind.AsteriskEqualsToken:
                    kind = SyntaxKind.MultiplyAssignmentExpression;
                    break;
                case SyntaxKind.SlashToken:
                    kind = SyntaxKind.DivideExpression;
                    break;
                case SyntaxKind.SlashEqualsToken:
                    kind = SyntaxKind.DivideAssignmentExpression;
                    break;
                case SyntaxKind.GreaterThanToken:
                    kind = SyntaxKind.GreaterThanExpression;
                    break;
                case SyntaxKind.GreaterThanEqualsToken:
                    kind = SyntaxKind.GreaterThanOrEqualExpression;
                    break;
                case SyntaxKind.LessThanToken:
                    kind = SyntaxKind.LessThanExpression;
                    break;
                case SyntaxKind.LessThanEqualsToken:
                    kind = SyntaxKind.LessThanOrEqualExpression;
                    break;
                case SyntaxKind.EqualsEqualsToken:
                    kind = SyntaxKind.EqualsExpression;
                    break;
                case SyntaxKind.ExclamationEqualsToken:
                    kind = SyntaxKind.NotEqualsExpression;
                    break;
                case SyntaxKind.EqualsToken:
                    kind = SyntaxKind.SimpleAssignmentExpression;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private static SyntaxToken GetUnaryOperator(string value, out SyntaxKind kind)
        {
            var result = CSharp.ParseToken(value);
            kind = result.Kind();
            switch (kind)
            {
                case SyntaxKind.EqualsToken:
                    kind = SyntaxKind.SimpleAssignmentExpression;
                    break;
            }

            return result;
        }

        private static ExpressionSyntax UnaryExpression(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 2);
            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(2));

            SyntaxKind kind;
            var op = GetUnaryOperator(node.children[0].GetText(), out kind);

            return CSharp.PrefixUnaryExpression(kind, op, expr);
        }

        private static ArgumentListSyntax Arguments(ParserRuleContext node)
        {
            ExpressionSyntax expr;
            var args = null as ArgumentListSyntax;
            if (node.ChildCount == 1)
            {
                expr = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            }
            else
            {
                Debug.Assert(node.ChildCount == 2);
                expr = VisitNode(node.GetRuleContext<ParserRuleContext>(1));
                args = Arguments(node.GetRuleContext<ParserRuleContext>(0));
            }

            var arg = CSharp.Argument(expr);
            if (args != null)
                return args.AddArguments(arg);

            return CSharp
                .ArgumentList(CSharp
                    .SeparatedList(new[] {arg}));
        }

        private static ExpressionSyntax Constant(ParserRuleContext node)
        {
            return CSharp.ParseExpression(node.GetText());
        }

        private static ExpressionSyntax Parenthesized(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 3);
            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(0));

            return CSharp.ParenthesizedExpression(expr);
        }

        private static ExpressionSyntax Cast(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 2);
            var type = CSharp.ParseTypeName(node.children[0].GetText());
            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(1));

            return CSharp.CastExpression(type, expr);
        }

        private static ExpressionSyntax BinaryExpression(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 3);
            var left = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            var right = VisitNode(node.GetRuleContext<ParserRuleContext>(1));

            SyntaxKind kind;
            var op = GetBinaryOperator(node.children[1].GetText(), out kind);

            return CSharp.BinaryExpression(kind, left, op, right);
        }

        private static ExpressionSyntax MemberAccess(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 3);
            var left = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            var right = CSharp.IdentifierName(node.children[2].GetText());

            SyntaxKind kind;
            var op = GetBinaryOperator(node.children[1].GetText(), out kind);
            return CSharp.MemberAccessExpression(kind, left, op, right);
        }

        private static ExpressionSyntax Index(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 4);
            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            var index = Arguments(node.GetRuleContext<ParserRuleContext>(2));

            return CSharp.ElementAccessExpression(
                expr, CSharp
                    .BracketedArgumentList(index.Arguments));
        }

        private static ExpressionSyntax Call(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(0));
            var args = null as ArgumentListSyntax;

            args = node.ChildCount == 4
                ? Arguments(node.GetRuleContext<ParserRuleContext>(2))
                : CSharp.ArgumentList();

            return CSharp.InvocationExpression(expr, args);
        }

        private static ExpressionSyntax PostFix(ParserRuleContext node)
        {
            if (node.ChildCount == 1)
                return Expression(node.GetRuleContext<ParserRuleContext>(0));

            Debug.Assert(node.ChildCount == 2);
            var expr = VisitNode(node.GetRuleContext<ParserRuleContext>(0));

            SyntaxKind kind;
            var op = GetBinaryOperator(node.children[1].GetText(), out kind);
            return CSharp.PostfixUnaryExpression(kind, expr, op);
        }

        private static ExpressionSyntax VisitNode(ParserRuleContext node)
        {
            var typename = node.GetType().Name;
            return Handlers[typename](node);
        }
    }
}