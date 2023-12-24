using SEx.Generic.Text;

namespace SEx.AST;

public sealed class ExpressionStatement : Statement
{
    public Expression Expression  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ExpressionStatement;

    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
        Span       = expression.Span;
    }

    public static ExpressionStatement Empty(Span span) => new(Expression.Unknown(span));

    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
    }
}
