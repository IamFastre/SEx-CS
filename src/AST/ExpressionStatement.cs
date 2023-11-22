using SEx.Generic.Text;
using SEx.Generic.Constants;

namespace SEx.AST;

internal sealed class ExpressionStatement : Statement
{
    public Expression Expression { get; }

    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
        Span       = expression.Span;
        Kind       = NodeKind.ExpressionStatement;
    }

    public static ExpressionStatement Empty(Span span) => new(Literal.Unknown(span));

    public override string ToString() => $"<{C.BLUE2}ExpressionStatement{C.GREEN2}[{Expression}]{C.END}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
    }
}
