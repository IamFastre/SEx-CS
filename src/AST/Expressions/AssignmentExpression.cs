using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

public sealed class AssignmentExpression : Expression
{
    public NameLiteral Assignee    { get; }
    public Token       Equal       { get; }
    public Token?      Operator    { get; }
    public Expression  Expression  { get; }

    public override Span     Span  { get; }
    public override NodeKind Kind => NodeKind.AssignmentExpression;

    public AssignmentExpression(NameLiteral name, Token equal, Expression expr)
    {
        Assignee   = name;
        Equal      = equal;
        Operator   = equal.Kind != TokenKind.Equal
                   ? new(equal.Value, GetOperationKind(equal.Kind), equal.Span)
                   : null;
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
        TokenKind.ANDEqual               => TokenKind.Ampersand,
        TokenKind.OREqual                => TokenKind.Pipe,
        TokenKind.XOREqual               => TokenKind.Caret,
        TokenKind.PowerEqual             => TokenKind.Power,
        TokenKind.LogicalANDEqual        => TokenKind.LogicalAND,
        TokenKind.LogicalOREqual         => TokenKind.LogicalOR,
        TokenKind.NullishCoalescingEqual => TokenKind.NullishCoalescing,

        _ => throw new Exception("if this happens imma kms"),
    };

    public override IEnumerable<Node> GetChildren()
    {
        yield return Assignee;
        yield return Equal.Node;
        yield return Expression;
    }
}