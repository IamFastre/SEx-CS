using SEx.Generic.Text;

namespace SEx.Semantics;

public sealed class SemanticWhileStatement : SemanticStatement
{
    public SemanticExpression  Condition  { get; }
    public SemanticStatement   Body       { get; }
    public SemanticElseClause? ElseClause { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.WhileStatement;

    public SemanticWhileStatement(SemanticExpression condition, SemanticStatement body, Span span, SemanticElseClause? elseClause = null)
    {
        Condition  = condition;
        Body       = body;
        ElseClause = elseClause;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Body;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
