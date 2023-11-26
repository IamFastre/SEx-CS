using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class ParenExpression : Expression
{
    public Token OpenParen        { get; }
    public Expression? Expression { get; }
    public Token CloseParen       { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ParenExpression;

    public ParenExpression(Token openParen, Expression? expression,Token closeParen)
    {
        OpenParen  = openParen;
        Expression = expression;
        CloseParen = closeParen;

        Span       = new Span(OpenParen.Span.Start, CloseParen.Span.End);
    }


    public override string ToString() => $"{C.RED}({C.END}{Expression}{C.RED}){C.END}";
    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen.Node;
        yield return Expression!;
        yield return CloseParen.Node;
    }
}
