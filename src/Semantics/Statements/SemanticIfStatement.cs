using SEx.Generic.Text;

namespace SEx.Semantics;

public sealed class SemanticIfStatement : SemanticStatement
{
    public SemanticExpression  Condition  { get; }
    public SemanticStatement   Then       { get; }
    public SemanticElseClause? ElseClause { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.IfStatement;

    public SemanticIfStatement(SemanticExpression condition, SemanticStatement body, Span span, SemanticElseClause? elseClause = null)
    {
        Condition  = condition;
        Then       = body;
        ElseClause = elseClause;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Then;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
