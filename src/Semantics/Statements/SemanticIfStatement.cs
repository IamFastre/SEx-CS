using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public sealed class SemanticIfStatement : SemanticStatement
{
    public SemanticExpression  Condition     { get; }
    public SemanticStatement   Then          { get; }
    public SemanticStatement?  ElseStatement { get; }

    public override Span         Span        { get; }
    public override SemanticKind Kind => SemanticKind.IfStatement;

    public SemanticIfStatement(SemanticExpression condition, SemanticStatement body, SemanticStatement? elseStmt, Span span)
    {
        Condition     = condition;
        Then          = body;
        ElseStatement = elseStmt;

        Span          = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Then;
        if (ElseStatement is not null)
            yield return ElseStatement;
    }
}
