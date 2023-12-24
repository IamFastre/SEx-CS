using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class ReturnStatement : Statement
{
    public Token      Return      { get; }
    public Expression Expression  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ReturnStatement;

    public ReturnStatement(Token @return, Expression expr)
    {
        Return     = @return;
        Expression = expr;

        Span       = new(@return.Span, expr.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Return.Node;
        yield return Expression;
    }
}