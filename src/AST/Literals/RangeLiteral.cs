using SEx.Generic.Text;

namespace SEx.AST;

internal sealed class RangeLiteral : Expression
{
    public Expression  Start { get; }
    public Expression  End   { get; }
    public Expression? Step  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.Range;

    public RangeLiteral(Expression start, Expression end, Expression? step)
    {
        Start = start;
        End   = end;
        Step  = step;
        Span  = new(start.Span, step is not null
                              ? step.Span
                              : end.Span);
    }

    public sealed override string ToString()
        => $"<{Start}:{End}{(Step is not null ? $":{Step}" : "")}>";

    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Start;
        yield return End;
        if (Step is not null)
            yield return Step;
    }
}
