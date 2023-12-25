using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class ReturnStatement : Statement
{
    public Token       Return      { get; }
    public Expression? Expression  { get; }

    public override Span     Span  { get; }
    public override NodeKind Kind => NodeKind.ReturnStatement;

    public ReturnStatement(Token @return, Expression? expr)
    {
        Return     = @return;
        Expression = expr;

        Span       = expr is null ? @return.Span : new(@return.Span, expr.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Return.Node;
        if (Expression is not null)
            yield return Expression;
    }
}