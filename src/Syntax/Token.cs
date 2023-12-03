using SEx.AST;
using SEx.Generic.Text;
using SEx.Generic.Constants;

namespace SEx.Lex;

// The Token class represents a token in a programming language,
// storing its value, type, and position in the source code
internal class Token
{
    public string    Value { get; }
    public Span      Span  { get; }
    public TokenKind Kind  { get; }
    public TokenNode Node  => new(this);

    public Token(string value, TokenKind kind, Position pos)
        : this(value, kind, new Span(pos)) {}

    public Token(string value, TokenKind kind, Span span)
    {
        Value = value;
        Kind  = kind;
        Span  = span;
    }

    public static Token Unknown(Span? span = null) => new(CONSTS.UNKNOWN, TokenKind.Unknown, span ?? new Span());

     public override string ToString()
     {
        var val = Kind.IsParserIgnorable() || Kind is TokenKind.EOF || Value is null
                ? "" : $": {C.GREEN2}{Value}";

        return $"[{C.YELLOW2}{Kind}{val}{C.END}]";
     }

}

internal class TokenNode : Node
{
    public Token  Token        { get; }
    public string Value        { get; }

    public override Span Span  { get; }
    public override NodeKind Kind => NodeKind.Token;


    public TokenNode(Token token)
    {
        Value     = token.Value;
        Span      = token.Span;
        Token = token;
    }

    public override IEnumerable<Node> GetChildren() { yield return this; }

    public override string ToString() => Token.ToString();
}

internal static class TokenExtension
{
    public static string ValueStrings(this Token[] tokens)
    {
        var str = string.Empty;
        foreach (var tk in tokens)
            str += tk == tokens.Last() ? tk.Value : $"{tk.Value}, ";

        return str;
    }
}
