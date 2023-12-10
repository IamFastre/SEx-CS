using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

internal class ConversionExpression : Expression
{
    public Expression Expression  { get; }
    public TypeClause Destination        { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ConversionExpression;

    public ConversionExpression(Expression expr, TypeClause type)
    {
        Expression = expr;
        Destination       = type;

        Span       = new(expr.Span, type.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
        yield return Destination;
    }
}