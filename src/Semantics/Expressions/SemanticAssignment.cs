using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping;

namespace SEx.Semantics;

internal sealed class SemanticAssignment : SemanticExpression
{
    public VariableSymbol        Assignee   { get; }
    public SemanticExpression    Expression { get; }

    public override ValType      Type       { get; }
    public override Span         Span       { get; }
    public override SemanticKind Kind => SemanticKind.AssignExpression;

    public SemanticAssignment(VariableSymbol assignee, SemanticExpression expr, Span span)
    {
        Assignee   = assignee;
        Expression = expr;

        Type = Expression.Type;
        Span = span;
    }

}