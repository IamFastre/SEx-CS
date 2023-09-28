using SEx.Generic;
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


    public override string ToString() => $"({Expression})";
    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen;
        yield return Expression!;
        yield return CloseParen;
    }
}
