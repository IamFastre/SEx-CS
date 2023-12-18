using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

public sealed class SemanticAssignment : SemanticExpression
{
    public SemanticName       Assignee   { get; }
    public SemanticExpression Expression { get; }
    public string             Operator   { get; }

    public override Span         Span    { get; }
    public override SemanticKind Kind => SemanticKind.AssignExpression;

    public SemanticAssignment(SemanticName assignee, SemanticExpression expr, Token? operation, Span span)
        : base(expr.Type)
    {
        Assignee   = assignee;
        Expression = expr;
        Operator   = operation?.Value ?? "=";
    
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Assignee;
        yield return Expression;
    }
}