using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticRange : SemanticExpression
{
    public SemanticExpression  Start { get; }
    public SemanticExpression  End   { get; }
    public SemanticExpression? Step  { get; }

    public override Span     Span { get; }
    public override ValType Type      => ValType.Range;
    public override SemanticKind Kind => SemanticKind.Range;

    public SemanticRange(SemanticExpression start, SemanticExpression end, SemanticExpression? step)
    {
        Start = start;
        End   = end;
        Step  = step;
        Span  = new(start.Span, step is not null
                              ? step.Span
                              : end.Span);
    }
}
