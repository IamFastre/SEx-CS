using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public class SemanticTernaryOperation : SemanticExpression
{
    public SemanticExpression Condition       { get; }
    public SemanticExpression TrueExpression  { get; }
    public SemanticExpression FalseExpression { get; }

    public override Span         Span         { get; }
    public override SemanticKind Kind => SemanticKind.TernaryOperation;

    public SemanticTernaryOperation(SemanticExpression condition, SemanticExpression trueExpr, SemanticExpression falseExpr, Span span)
        : base(trueExpr.Type == falseExpr.Type ? trueExpr.Type : TypeSymbol.Unknown)
    {
        Condition       = condition;
        TrueExpression  = trueExpr;
        FalseExpression = falseExpr;

        Span            = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return TrueExpression;
        yield return FalseExpression;
    }
}