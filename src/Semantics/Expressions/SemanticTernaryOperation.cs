using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal class SemanticTernaryOperation : SemanticExpression
{
    public SemanticExpression Condition       { get; }
    public SemanticExpression TrueExpression  { get; }
    public SemanticExpression FalseExpression { get; }

    public override Span    Span { get; }
    public override ValType Type { get; }
    public override SemanticKind Kind => SemanticKind.TernaryOperation;

    public SemanticTernaryOperation(SemanticExpression condition, SemanticExpression trueExpr, SemanticExpression falseExpr)
    {
        Condition       = condition;
        TrueExpression  = trueExpr;
        FalseExpression = falseExpr;

        Type            = TrueExpression.Type == FalseExpression.Type
                        ? TrueExpression.Type
                        : ValType.Unknown;
        Span            = new(condition.Span, falseExpr.Span);
    }
}