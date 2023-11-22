using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class BinaryExpression : Expression
{
    public Expression LHS;
    public Token Operator;
    public Expression RHS;

    public BinaryExpression(Expression leftHandExpr, Token binOperator, Expression rightHandExpr)
    {
        LHS = leftHandExpr;
        Operator = binOperator;
        RHS = rightHandExpr;

        Span = new Span(LHS.Span.Start, RHS.Span.End);
        Kind = NodeKind.BinaryOperation;
    }

    public override string ToString() => $"<{C.BLUE2}BinaryOperation{C.END}: {LHS} {C.GREEN2}{Operator.Value}{C.END} {RHS}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return LHS;
        yield return Operator;
        yield return RHS;
    }
}
