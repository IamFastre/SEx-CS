using SEx.Generic.Text;

namespace SEx.Semantics;

public sealed class SemanticAssignment : SemanticExpression
{
    public SemanticName       Assignee   { get; }
    public SemanticExpression Expression { get; }
    public string             Operator   { get; }
    public bool               IsCompound { get; }

    public override Span         Span    { get; }
    public override SemanticKind Kind => SemanticKind.AssignExpression;

    public SemanticAssignment(SemanticName assignee, SemanticExpression expr, string? operation, Span span)
        : base(expr.Type)
    {
        Assignee   = assignee;
        Expression = expr;
        Operator   = operation ?? "=";
        IsCompound = operation is not null;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Assignee;
        yield return Expression;
    }
}