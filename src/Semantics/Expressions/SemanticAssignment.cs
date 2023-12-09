using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticAssignment : SemanticExpression
{
    public SemanticVariable      Assignee   { get; }
    public SemanticExpression    Expression { get; }
    public string                 Operator   { get; }

    public override Span         Span       { get; }
    public override TypeSymbol   Type       { get; }
    public override SemanticKind Kind => SemanticKind.AssignExpression;

    public SemanticAssignment(SemanticVariable assignee, SemanticExpression expr, Token? operation, Span span)
    {
        Assignee   = assignee;
        Expression = expr;
        Operator   = operation?.Value ?? "=";
    
        Type       = Expression.Type;
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Assignee;
        yield return Expression;
    }
}