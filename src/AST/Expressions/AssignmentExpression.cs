using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal class AssignmentExpression : Expression
{
    public NameLiteral       Assignee    { get; }
    public Token      Equal       { get; }
    public Expression Expression  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.AssignmentExpression;

    public AssignmentExpression(NameLiteral name, Token equal, Expression expr)
    {
        Assignee   = name;
        Equal      = equal;
        Expression = expr;

        Span       = new Span(Assignee.Span, Expression.Span);
    }

    public override string ToString() => $"<{C.BLUE2}Assignment{C.END}: {Assignee} {C.BLUE2}=>{C.END} {Expression}{C.END}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Assignee;
        yield return Equal.Node;
        yield return Expression;
    }
}
