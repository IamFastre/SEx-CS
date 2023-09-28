using SEx.Generic;
using SEx.Lex;

namespace SEx.AST;

public sealed class BinaryExpression : Expression
{
    public Expression LHS;
    public Token Operator;
    public Expression RHS;

    public BinaryExpression(Expression leftHandExpr, Token binOperator, Expression rightHandExpr)
    {
        LHS = leftHandExpr;
        Operator = binOperator;
        RHS = rightHandExpr;

        Span = new Span(LHS.Span!.Start, RHS.Span!.End);
        Kind = NodeKind.BinaryOperation;
    }

    public override string ToString() => $"<Binary Operation: {LHS} {Operator.Value} {RHS}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return LHS;
        yield return Operator;
        yield return RHS;
    }
}
