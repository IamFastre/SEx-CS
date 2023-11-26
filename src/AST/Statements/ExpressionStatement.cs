using SEx.Generic.Text;
using SEx.Generic.Constants;

namespace SEx.AST;

internal sealed class ExpressionStatement : Statement
{
    public Expression Expression  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ExpressionStatement;

    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
        Span       = expression.Span;
    }

    public static ExpressionStatement Empty(Span span) => new(Literal.Unknown(span));

    public override string ToString() => $"<{C.BLUE2}ExpressionStatement{C.GREEN2}[{C.END}{Expression}{C.GREEN2}]{C.END}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
    }
}
