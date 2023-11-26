using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class CompoundAssignmentExpression : Expression
{
    public Name       Assignee    { get; }
    public Token      Operator    { get; }
    public Expression Expression  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.CompoundAssignmentExpression;

    public CompoundAssignmentExpression(Name name, Token equal, Expression expr)
    {
        Assignee   = name;
        Operator   = new(equal.Value, GetOperationKind(equal.Kind), equal.Span);
        Expression = expr;
        Span       = new(name.Span, expr.Span);
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
        yield return Operator.Node;
        yield return Expression;
    }
}