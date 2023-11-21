using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public sealed class Name : Expression
{
    public Token  Token { get; }
    public string Value { get; }

    public Name(Token token, NodeKind kind)
    {
        Token = token;
        Value = token.Value;
        Span  = Token.Span;
        Kind  = kind;
    }

    public sealed override string ToString() => $"<{C.YELLOW2}{Kind}{C.END}: {C.GREEN2}{Value}{C.END}>";
    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Token;
    }
}