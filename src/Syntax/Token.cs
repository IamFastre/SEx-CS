using SEx.AST;
using SEx.Generic;
using SEx.Generic.Constants;

namespace SEx.Lex;

// The Token class represents a token in a programming language,
// storing its value, type, and position in the source code
public class Token : Node
{
    public string Value = "";
    public new TokenKind Kind { get; }
    public new Span Span;

    public Token(string value, TokenKind kind, Span span)
    {
        Value = value;
        Kind  = kind;
        Span  = span;
    }

    public static readonly Token Template = new(CONSTS.NULL, TokenKind.Null, Span.Template);

     public override string ToString()
     {
        TokenKind[] noVal = {TokenKind.WhiteSpace, TokenKind.EOF};

        var val = !noVal.Contains(Kind) || Value == null ? $": {C.GREEN2}{Value}" : "";

        return $"[{C.YELLOW2}{Kind}{val}{C.END}]";
     }

    public override IEnumerable<Node> GetChildren() { yield return this; }

    public string Full => $"{this} at {Span}";
}
