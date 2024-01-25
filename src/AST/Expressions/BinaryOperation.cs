using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class BinaryOperation : Expression
{
    public Expression LHS         { get; }
    public Token      Operator    { get; }
    public Expression RHS         { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.BinaryOperation;

    public BinaryOperation(Expression leftHandExpr, Token binOperator, Expression rightHandExpr)
    {
        LHS      = leftHandExpr;
        Operator = binOperator;
        RHS      = rightHandExpr;

        Span     = new Span(LHS.Span.Start, RHS.Span.End);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return LHS;
        yield return Operator.Node;
        yield return RHS;
    }
}
