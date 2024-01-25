using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public sealed class SemanticExpressionStatement : SemanticStatement
{
    public SemanticExpression Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.ExpressionStatement;

    public SemanticExpressionStatement(SemanticExpression expression)
    {
        Expression = expression;
        Span       = expression.Span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Expression;
    }
}
