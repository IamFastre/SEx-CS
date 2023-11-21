using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public sealed class ParenExpression : Expression
{
    public Token OpenParen;
    public Expression? Expression;
    public Token CloseParen;

    public ParenExpression(Token openParen, Expression? expression,Token closeParen)
    {
        OpenParen = openParen;
        Expression = expression;
        CloseParen = closeParen;

        Span = new Span(OpenParen.Span.Start, CloseParen.Span.End);
        Kind = NodeKind.ParenExpression;
    }


    public override string ToString() => $"{C.RED}({C.END}{Expression}{C.RED}){C.END}";
    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen;
        yield return Expression!;
        yield return CloseParen;
    }
}
