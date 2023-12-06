using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticParenExpression : SemanticExpression
{
    public Token               OpenParen  { get; }
    public SemanticExpression? Expression { get; }
    public Token               CloseParen { get; }

    public override Span Span { get; }
    public override TypeSymbol   Type => Expression?.Type ?? TypeSymbol.Null;
    public override SemanticKind Kind => SemanticKind.ParenExpression;

    public SemanticParenExpression(Token openParen, SemanticExpression? expression, Token closeParen)
    {
        OpenParen  = openParen;
        Expression = expression;
        CloseParen = closeParen;

        Span = new(openParen.Span.Start, closeParen.Span.End);
    }
}
