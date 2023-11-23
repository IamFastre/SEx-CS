namespace SEx.Lex;

// An enumeration of the token types possible in the source code
public enum TokenKind
{
    // Special kinds
    EOF,
    Unknown,
    Comment,
    NewLine,
    WhiteSpace,
    BigWhiteSpace,

    // Literal kinds
    Boolean,
    Integer,
    Float,
    Char,
    String,

    // Identifier kinds
    Identifier,
    Keyword,
    Type,
    InOperator,
    Null,

    // Operational kinds
    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Percent,
    Power,

    AND,
    OR,
    XOR,
    IsEqual,
    NotEqual,
    LogicalAND,
    LogicalOR,
    NullishCoalescing,
    // Unary
    Increment,
    Decrement,
    ExclamationMark,

    Delete,

    // Assignment kinds
    __ASSIGNMENT_START__,
    Equal,
    PlusEqual,
    MinusEqual,
    AsteriskEqual,
    ForwardSlashEqual,
    PercentEqual,
    ANDEqual,
    OREqual,
    XOREqual,
    PowerEqual,
    LogicalANDEqual,
    LogicalOREqual,
    NullishCoalescingEqual,
    __ASSIGNMENT_END__,

    // Bracket kinds
    OpenParenthesis,
    CloseParenthesis,
    OpenCurlyBracket,
    CloseCurlyBracket,
    OpenSquareBracket,
    CloseSquareBracket,

    // Punctuational kinds
    Dot,
    Comma,
    Colon,
    Semicolon,
    DollarSign,
    Hash,
    QuestionMark,

    // Other kinds
    Separator,
}

internal static class TokenKindExtension
{
    // private static readonly TokenKind[]  = {  };

    private static readonly TokenKind[] or = { TokenKind.OR, TokenKind.XOR, TokenKind.LogicalOR };
    private static readonly TokenKind[] and = { TokenKind.AND, TokenKind.LogicalAND };
    private static readonly TokenKind[] additives = { TokenKind.Plus, TokenKind.Minus };
    private static readonly TokenKind[] multiplicatives = { TokenKind.Asterisk, TokenKind.ForwardSlash, TokenKind.Percent, TokenKind.Power };
    private static readonly TokenKind[] comparatives = { TokenKind.IsEqual, TokenKind.NotEqual, TokenKind.InOperator };
    private static readonly TokenKind[] ignorables = { TokenKind.WhiteSpace, TokenKind.BigWhiteSpace, TokenKind.Comment, TokenKind.Unknown };
    private static readonly TokenKind[] eos = { TokenKind.Semicolon, TokenKind.EOF };

    // public static bool Is (this TokenKind kind) => .Contains(kind);
    public static bool IsOR(this TokenKind kind)              => or.Contains(kind);
    public static bool IsAND(this TokenKind kind)             => and.Contains(kind);
    public static bool IsAdditive(this TokenKind kind)        => additives.Contains(kind);
    public static bool IsMultiplicative(this TokenKind kind)  => multiplicatives.Contains(kind);
    public static bool IsComparative(this TokenKind kind)     => comparatives.Contains(kind);
    public static bool IsAssignment(this TokenKind kind)      => TokenKind.__ASSIGNMENT_START__ < kind
                                                              && TokenKind.__ASSIGNMENT_END__   > kind;
    public static bool IsParserIgnorable(this TokenKind kind) => ignorables.Contains(kind);
    public static bool IsEOS(this TokenKind kind) => eos.Contains(kind);

    public static int UnaryPrecedence(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Plus      or
            TokenKind.Minus     or
            TokenKind.Increment or
            TokenKind.Decrement or
            TokenKind.ExclamationMark => 6,

            _ => 0,
    };
    }

    public static int BinaryPrecedence(this TokenKind kind)
    {
        if (kind.IsMultiplicative())
            return 6;
        if (kind.IsAdditive())
            return 5;
        if (kind.IsComparative())
            return 4;
        if (kind.IsAND())
            return 3;
        if (kind.IsOR())
            return 2;
        if (kind is TokenKind.NullishCoalescing)
            return 1;
        return 0;
    }
}