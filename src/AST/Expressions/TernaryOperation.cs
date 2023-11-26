using SEx.Generic.Constants;
using SEx.Generic.Text;

namespace SEx.AST;

internal class TernaryOperation : Expression
{
    public Expression Condition       { get; }
    public Expression TrueExpression  { get; }
    public Expression FalseExpression { get; }

    public override Span Span { get; }
    public override NodeKind Kind => NodeKind.TernaryOperation;

    public TernaryOperation(Expression condition, Expression trueExpr, Expression falseExpr)
    {
        Condition       = condition;
        TrueExpression  = trueExpr;
        FalseExpression = falseExpr;

        Span            = new(condition.Span, falseExpr.Span);
    }

    public override string ToString()
        => $"<{C.BLUE2}TernaryOperation{C.END}: {Condition} {C.GREEN2}?{C.END} {TrueExpression} {C.GREEN2}:{C.END} {FalseExpression}>";

    public override IEnumerable<Node> GetChildren()
    {
        yield return Condition;
        yield return TrueExpression;
        yield return FalseExpression;
    }
}