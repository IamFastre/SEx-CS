using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class CallExpression : Expression
{
    public Expression                   Function    { get; }
    public Token                        OpenParen   { get; }
    public SeparatedClause<Expression>  Arguments   { get; }
    public Token                        CloseParen  { get; }

    public override Span     Span                   { get; }
    public override NodeKind Kind => NodeKind.CallExpression;

    public CallExpression(Expression func, Token openParen, SeparatedClause<Expression> arguments, Token closeParen)
    {
        Function   = func;
        OpenParen  = openParen;
        Arguments  = arguments;
        CloseParen = closeParen;

        Span       = new(func.Span, closeParen.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Function;
        yield return OpenParen.Node;
        yield return Arguments;
        yield return CloseParen.Node;
    }
}
