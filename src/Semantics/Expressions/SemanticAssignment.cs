using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticAssignment : SemanticExpression
{
    public Name               Assignee   { get; }
    public Token              Equal      { get; }
    public SemanticExpression Expression { get; }

    public override ValType Type { get; }
    public override Span    Span { get; }
    public override SemanticKind Kind => SemanticKind.AssignExpression;

    public SemanticAssignment(Name assignee, Token equal, SemanticExpression expr)
    {
        Assignee   = assignee;
        Equal      = equal;
        Expression = expr;

        Span = new Span(assignee.Span.Start, expr.Span.End);
        Type = Expression.Type;
    }

}