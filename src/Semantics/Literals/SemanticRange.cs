using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticRange : SemanticExpression
{
    public SemanticExpression  Start { get; }
    public SemanticExpression  End   { get; }
    public SemanticExpression? Step  { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.Range;

    public SemanticRange(SemanticExpression start, SemanticExpression end, SemanticExpression? step)
        : base(TypeSymbol.Range)
    {
        Start = start;
        End   = end;
        Step  = step;
        Span  = new(start.Span, step is not null
                              ? step.Span
                              : end.Span);
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Start;
        yield return End;
        if (Step is not null)
            yield return Step;
    }
}
