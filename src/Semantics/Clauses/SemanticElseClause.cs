using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public sealed class SemanticElseClause : SemanticClause
{
    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.ElseClause;

    public SemanticStatement Body  { get; }

    public SemanticElseClause(SemanticStatement body, Span span)
    {
        Body = body;
        Span = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Body;
    }
}
