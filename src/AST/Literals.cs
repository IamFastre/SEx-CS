using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class Literal : Expression
{
    public Token  Token { get; }
    public string Value { get; }

    public Literal(Token token, NodeKind kind)
    {
        Token = token;
        Value = token.Value;
        Span  = Token.Span;
        Kind  = kind;
    }

    public sealed override string ToString() => $"<{C.YELLOW2}{Kind}{C.END}: {C.GREEN2}{Value}{C.END}>";
    public static Literal Unknown(Span? span = null) => new(Token.Unknown(span), NodeKind.Unknown);

    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Token;
    }
}
