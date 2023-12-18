using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

public sealed class Literal : Expression
{
    public Token  Token           { get; }
    public string Value           { get; }

    public override Span     Span { get; }
    public override NodeKind Kind { get; }

    public Literal(Token token, NodeKind kind)
    {
        Token = token;
        Value = token.Value;
        Span  = Token.Span;
        Kind  = kind;
    }

    public static Literal Unknown(Span? span = null) => new(Token.Unknown(span), NodeKind.Unknown);

    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Token.Node;
    }
}
