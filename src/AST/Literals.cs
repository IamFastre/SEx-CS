using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public abstract class Literal : Expression
{
    public Token? Token;
    public string Value = "";

    public static readonly UnknownLiteral Unknown = new(Token.Template);

    public sealed override string ToString() => $"{Kind}: {Token!.Value}";
    public string Full => $"{this} at {Span}";

    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Token!;
    }
}

public sealed class UnknownLiteral : Literal
{
    public UnknownLiteral(Token token)
    {
        Token = token;
        Value = CONSTS.NULL;
        Span  = Token.Span;
        Kind  = NodeKind.Unknown;
    }
}

public sealed class NullLiteral : Literal
{
    public NullLiteral(Token token)
    {
        Token = token;
        Value = CONSTS.NULL;
        Span  = token.Span;
        Kind  = NodeKind.Null;
    }
}

public sealed class BooleanLiteral : Literal
{
    public BooleanLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span  = token.Span;
        Kind  = NodeKind.Boolean;
    }
}

public sealed class IntLiteral : Literal
{
    public IntLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span  = token.Span;
        Kind  = NodeKind.Integer;
    }
}

public sealed class FloatLiteral : Literal
{
    public FloatLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span  = token.Span;
        Kind  = NodeKind.Float;
    }
}

public sealed class CharLiteral : Literal
{
    public CharLiteral(Token token)
    {
        Token = token;
        Value = token.Value![1..^1];
        Span  = token.Span;
        Kind  = NodeKind.Char;
    }
}

public sealed class StringLiteral : Literal
{
    public StringLiteral(Token token)
    {
        Token = token;
        Value = token.Value![1..^1];
        Span  = token.Span;
        Kind  = NodeKind.String;
    }
}

public sealed class IdentifierLiteral : Literal
{
    public IdentifierLiteral(Token token)
    {
        Token = token;
        Value = token.Value!.ToString();
        Span  = token.Span;
        Kind  = NodeKind.Identifier;
    }
}
