//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from R.g4 by ANTLR 4.5

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="RParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5")]
[System.CLSCompliant(false)]
public interface IRListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="RParser.prog"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProg([NotNull] RParser.ProgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="RParser.prog"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProg([NotNull] RParser.ProgContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Assignment</c>
	/// labeled alternative in <see cref="RParser.expr_or_assign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment([NotNull] RParser.AssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Assignment</c>
	/// labeled alternative in <see cref="RParser.expr_or_assign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment([NotNull] RParser.AssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ExpressionStatement</c>
	/// labeled alternative in <see cref="RParser.expr_or_assign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionStatement([NotNull] RParser.ExpressionStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ExpressionStatement</c>
	/// labeled alternative in <see cref="RParser.expr_or_assign"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionStatement([NotNull] RParser.ExpressionStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>HexLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterHexLiteral([NotNull] RParser.HexLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>HexLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitHexLiteral([NotNull] RParser.HexLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>NextStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNextStatement([NotNull] RParser.NextStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>NextStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNextStatement([NotNull] RParser.NextStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>IfElseStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfElseStatement([NotNull] RParser.IfElseStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>IfElseStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfElseStatement([NotNull] RParser.IfElseStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>NullLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullLiteral([NotNull] RParser.NullLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>NullLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullLiteral([NotNull] RParser.NullLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>MemberAccess</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberAccess([NotNull] RParser.MemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>MemberAccess</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberAccess([NotNull] RParser.MemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Parenthesized</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenthesized([NotNull] RParser.ParenthesizedContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Parenthesized</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenthesized([NotNull] RParser.ParenthesizedContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Compound</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompound([NotNull] RParser.CompoundContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Compound</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompound([NotNull] RParser.CompoundContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FloatLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFloatLiteral([NotNull] RParser.FloatLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FloatLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFloatLiteral([NotNull] RParser.FloatLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>WhileStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] RParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>WhileStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] RParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Formulae</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormulae([NotNull] RParser.FormulaeContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Formulae</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormulae([NotNull] RParser.FormulaeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>IntLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntLiteral([NotNull] RParser.IntLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>IntLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntLiteral([NotNull] RParser.IntLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ListAccess</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListAccess([NotNull] RParser.ListAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ListAccess</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListAccess([NotNull] RParser.ListAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>LogicalOr</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalOr([NotNull] RParser.LogicalOrContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>LogicalOr</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalOr([NotNull] RParser.LogicalOrContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>LogicalAnd</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalAnd([NotNull] RParser.LogicalAndContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>LogicalAnd</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalAnd([NotNull] RParser.LogicalAndContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Comparison</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComparison([NotNull] RParser.ComparisonContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Comparison</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComparison([NotNull] RParser.ComparisonContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Help</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterHelp([NotNull] RParser.HelpContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Help</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitHelp([NotNull] RParser.HelpContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ForEachStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForEachStatement([NotNull] RParser.ForEachStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ForEachStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForEachStatement([NotNull] RParser.ForEachStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Sequence</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSequence([NotNull] RParser.SequenceContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Sequence</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSequence([NotNull] RParser.SequenceContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Identifier</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdentifier([NotNull] RParser.IdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Identifier</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdentifier([NotNull] RParser.IdentifierContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Addition</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAddition([NotNull] RParser.AdditionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Addition</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAddition([NotNull] RParser.AdditionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>StringLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringLiteral([NotNull] RParser.StringLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>StringLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringLiteral([NotNull] RParser.StringLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Multiplication</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiplication([NotNull] RParser.MultiplicationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Multiplication</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiplication([NotNull] RParser.MultiplicationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>RightAssignment</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRightAssignment([NotNull] RParser.RightAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>RightAssignment</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRightAssignment([NotNull] RParser.RightAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Power</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPower([NotNull] RParser.PowerContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Power</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPower([NotNull] RParser.PowerContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>RepeatStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatement([NotNull] RParser.RepeatStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>RepeatStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatement([NotNull] RParser.RepeatStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>NanLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNanLiteral([NotNull] RParser.NanLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>NanLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNanLiteral([NotNull] RParser.NanLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FalseLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFalseLiteral([NotNull] RParser.FalseLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FalseLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFalseLiteral([NotNull] RParser.FalseLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Function</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunction([NotNull] RParser.FunctionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Function</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunction([NotNull] RParser.FunctionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>BreakStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] RParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>BreakStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] RParser.BreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ComplexLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexLiteral([NotNull] RParser.ComplexLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ComplexLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexLiteral([NotNull] RParser.ComplexLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FunctionCall</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCall([NotNull] RParser.FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FunctionCall</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCall([NotNull] RParser.FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>InfLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInfLiteral([NotNull] RParser.InfLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>InfLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInfLiteral([NotNull] RParser.InfLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>TrueLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTrueLiteral([NotNull] RParser.TrueLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>TrueLiteral</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTrueLiteral([NotNull] RParser.TrueLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Index</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIndex([NotNull] RParser.IndexContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Index</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIndex([NotNull] RParser.IndexContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>IfStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatement([NotNull] RParser.IfStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>IfStatement</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatement([NotNull] RParser.IfStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>UserOp</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUserOp([NotNull] RParser.UserOpContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>UserOp</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUserOp([NotNull] RParser.UserOpContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Sign</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSign([NotNull] RParser.SignContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Sign</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSign([NotNull] RParser.SignContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Namespace</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamespace([NotNull] RParser.NamespaceContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Namespace</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamespace([NotNull] RParser.NamespaceContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>NA</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNA([NotNull] RParser.NAContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>NA</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNA([NotNull] RParser.NAContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Negation</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNegation([NotNull] RParser.NegationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Negation</c>
	/// labeled alternative in <see cref="RParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNegation([NotNull] RParser.NegationContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ExpressionList</c>
	/// labeled alternative in <see cref="RParser.exprlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionList([NotNull] RParser.ExpressionListContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ExpressionList</c>
	/// labeled alternative in <see cref="RParser.exprlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionList([NotNull] RParser.ExpressionListContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Empty</c>
	/// labeled alternative in <see cref="RParser.exprlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmpty([NotNull] RParser.EmptyContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Empty</c>
	/// labeled alternative in <see cref="RParser.exprlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmpty([NotNull] RParser.EmptyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="RParser.formlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormlist([NotNull] RParser.FormlistContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="RParser.formlist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormlist([NotNull] RParser.FormlistContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FormIdentifier</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormIdentifier([NotNull] RParser.FormIdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FormIdentifier</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormIdentifier([NotNull] RParser.FormIdentifierContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FormAssignment</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormAssignment([NotNull] RParser.FormAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FormAssignment</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormAssignment([NotNull] RParser.FormAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FormEllipsis</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFormEllipsis([NotNull] RParser.FormEllipsisContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FormEllipsis</c>
	/// labeled alternative in <see cref="RParser.form"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFormEllipsis([NotNull] RParser.FormEllipsisContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="RParser.sublist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSublist([NotNull] RParser.SublistContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="RParser.sublist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSublist([NotNull] RParser.SublistContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubExpression</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubExpression([NotNull] RParser.SubExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubExpression</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubExpression([NotNull] RParser.SubExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubIncompleteAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubIncompleteAssignment([NotNull] RParser.SubIncompleteAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubIncompleteAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubIncompleteAssignment([NotNull] RParser.SubIncompleteAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubAssignment([NotNull] RParser.SubAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubAssignment([NotNull] RParser.SubAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubIncompleteString</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubIncompleteString([NotNull] RParser.SubIncompleteStringContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubIncompleteString</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubIncompleteString([NotNull] RParser.SubIncompleteStringContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubStringAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubStringAssignment([NotNull] RParser.SubStringAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubStringAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubStringAssignment([NotNull] RParser.SubStringAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubIncompleteNull</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubIncompleteNull([NotNull] RParser.SubIncompleteNullContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubIncompleteNull</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubIncompleteNull([NotNull] RParser.SubIncompleteNullContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubNullAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubNullAssignment([NotNull] RParser.SubNullAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubNullAssignment</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubNullAssignment([NotNull] RParser.SubNullAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubEllipsis</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubEllipsis([NotNull] RParser.SubEllipsisContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubEllipsis</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubEllipsis([NotNull] RParser.SubEllipsisContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>SubEmpty</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubEmpty([NotNull] RParser.SubEmptyContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>SubEmpty</c>
	/// labeled alternative in <see cref="RParser.sub"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubEmpty([NotNull] RParser.SubEmptyContext context);
}
