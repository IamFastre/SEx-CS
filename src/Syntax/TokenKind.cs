namespace SEx.Lex;

// An enumeration of the token types possible in the source code
public enum TokenKind
{
    // Special kinds
    EOF,
    __IGNORABLE_START__,
    Unknown,
    Comment,
    NewLine,
    WhiteSpace,
    BigWhiteSpace,
    __IGNORABLE_END__,

    // Literal kinds
    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String,
    StringFragment,
    Identifier,

    // Keyword kinds
    Keyword,
    Type,
    If,
    Else,
    While,
    For,
    Continue,
    Break,
    Return,

    // Operational kinds
    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Percent,
    Power,

    Tilde,
    Ampersand,
    Pipe,
    Caret,

    __COMPARATIVE_START__,
    EqualEqual,
    NotEqual,
    Greater,
    Less,
    GreaterEqual,
    LessEqual,
    InOperator,
    __COMPARATIVE_END__,

    LogicalAND,
    LogicalOR,
    NullishCoalescing,
    // Unary
    Increment,
    Decrement,
    BangMark,

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
    QuestionMark,
    // Others
    Hash,
    Ellipsis,
    DashArrow,
    EqualArrow,
    FunctionSymbol,

    // Other kinds
    Separator,
    FormatStringOpener,
    FormatStringCloser,
}

internal static class TokenKindExtension
{
    // private static readonly TokenKind[]  = {  };

    private static readonly TokenKind[] or              = { TokenKind.Pipe, TokenKind.Caret, TokenKind.LogicalOR };
    private static readonly TokenKind[] and             = { TokenKind.Ampersand, TokenKind.LogicalAND };
    private static readonly TokenKind[] additives       = { TokenKind.Plus, TokenKind.Minus };
    private static readonly TokenKind[] multiplicatives = { TokenKind.Asterisk, TokenKind.ForwardSlash, TokenKind.Percent, TokenKind.Power };
    private static readonly TokenKind[] eos             = { TokenKind.Semicolon, TokenKind.EOF };

    // public static bool Is (this TokenKind kind) => .Contains(kind);
    public static bool IsOR(this TokenKind kind)              => or.Contains(kind);
    public static bool IsAND(this TokenKind kind)             => and.Contains(kind);
    public static bool IsAdditive(this TokenKind kind)        => additives.Contains(kind);
    public static bool IsMultiplicative(this TokenKind kind)  => multiplicatives.Contains(kind);

    public static bool IsCounting(this TokenKind kind)        => kind is TokenKind.Increment
                                                                      or TokenKind.Decrement;

    public static bool IsComparative(this TokenKind kind)     => TokenKind.__COMPARATIVE_START__ < kind
                                                              && TokenKind.__COMPARATIVE_END__   > kind;

    public static bool IsAssignment(this TokenKind kind)      => TokenKind.__ASSIGNMENT_START__ < kind
                                                              && TokenKind.__ASSIGNMENT_END__   > kind;

    public static bool IsParserIgnorable(this TokenKind kind) => TokenKind.__IGNORABLE_START__ < kind
                                                              && TokenKind.__IGNORABLE_END__   > kind;

    public static bool IsEOS(this TokenKind kind) => eos.Contains(kind);

    public static int UnaryPrecedence(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Increment or
            TokenKind.Decrement => 7,

            TokenKind.Plus      or
            TokenKind.Minus     or
            TokenKind.Tilde     or
            TokenKind.BangMark => 6,

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