using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public class AssignmentExpression : Expression
{
    public Name Assignee;
    public Token Equal;
    public Expression Expression;

    public AssignmentExpression(Name name, Token equal, Expression expr)
    {
        Assignee   = name;
        Equal      = equal;
        Expression = expr;

        Span = new Span(Assignee.Span.Start, Expression.Span.End);
        Kind = NodeKind.AssignmentExpression;
    }

    public override string ToString() => $"<Assignment: {Assignee} => {Expression}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Assignee;
        yield return Equal;
        yield return Expression;
    }
}


internal sealed class CompoundAssignmentExpression : Expression
{
    public Name Assignee;
    public Token Operator;
    public Expression Expression;

    public CompoundAssignmentExpression(Name name, Token equal, Expression expr)
    {
        Assignee   = name;
        Operator   = new Token(equal.Value, GetOperationKind(equal.Kind), equal.Span);
        Expression = expr;
        Kind     = NodeKind.CompoundAssignmentExpression;
    }

    private static TokenKind GetOperationKind(TokenKind kind) => kind switch
    {
        TokenKind.PlusEqual              => TokenKind.Plus,
        TokenKind.MinusEqual             => TokenKind.Minus,
        TokenKind.AsteriskEqual          => TokenKind.Asterisk,
        TokenKind.ForwardSlashEqual      => TokenKind.ForwardSlash,
        TokenKind.PercentEqual           => TokenKind.Percent,
        TokenKind.ANDEqual               => TokenKind.AND,
        TokenKind.OREqual                => TokenKind.OR,
        TokenKind.XOREqual               => TokenKind.XOR,
        TokenKind.PowerEqual             => TokenKind.Power,
        TokenKind.LogicalANDEqual        => TokenKind.LogicalAND,
        TokenKind.LogicalOREqual         => TokenKind.LogicalOR,
        TokenKind.NullishCoalescingEqual => TokenKind.NullishCoalescing,

        _ => throw new Exception("Unknown assignment kind"),
    };

    public override string ToString() => $"<{C.BLUE2}CompoundAssignment{C.END}: {Assignee} {C.GREEN2}{Operator.Value}{C.END} {Expression}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Assignee;
        yield return Operator;
        yield return Expression;
    }
}