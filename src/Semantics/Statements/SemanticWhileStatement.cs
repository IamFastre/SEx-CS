using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public sealed class SemanticWhileStatement : SemanticStatement
{
    public SemanticExpression Condition     { get; }
    public SemanticStatement  Body          { get; }
    public SemanticStatement? ElseStatement { get; }

    public override Span         Span       { get; }
    public override SemanticKind Kind => SemanticKind.WhileStatement;

    public SemanticWhileStatement(SemanticExpression condition, SemanticStatement body, SemanticStatement? elseStmt, Span span)
    {
        Condition     = condition;
        Body          = body;
        ElseStatement = elseStmt;

        Span          = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Body;
        if (ElseStatement is not null)
            yield return ElseStatement;
    }
}
