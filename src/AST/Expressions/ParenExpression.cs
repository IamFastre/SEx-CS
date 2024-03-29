using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class ParenthesizedExpression : Expression
{
    public Token       OpenParen  { get; }
    public Expression? Expression { get; }
    public Token       CloseParen { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ParenthesizedExpression;

    public ParenthesizedExpression(Token open, Expression? expression, Token close)
    {
        OpenParen  = open;
        Expression = expression;
        CloseParen = close;

        Span       = new Span(OpenParen.Span.Start, CloseParen.Span.End);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen.Node;
        yield return Expression!;
        yield return CloseParen.Node;
    }
}
