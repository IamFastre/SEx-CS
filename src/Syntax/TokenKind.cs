namespace SEx.Lex;

// An enumeration of the token types possible in the source code
public enum TokenKind
{
    // Special kinds
    EOF,
    Unknown,
    WhiteSpace,
    Comment,

    // Literal kinds
    Integer,
    Float,
    Char,
    String,

    // Identifier kinds
    Identifier,
    Keyword,
    Boolean,
    Null,

    // Operational kinds
    Equal,
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
    AssignmentOperator,
    LogicalAND,
    LogicalOR,
    LogicalXOR,
    NullishCoalescing,
    // Unary
    Increment,
    Decrement,
    ExclamationMark,

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

    // Other kinds
    Separator,
}

internal static class TokenKindExtension
{
    // private static readonly TokenKind[]  = {  };
    private static readonly TokenKind[] literals = {
        TokenKind.Integer, TokenKind.Float, TokenKind.Char, TokenKind.String };

    private static readonly TokenKind[] operators = {
        TokenKind.Equal, TokenKind.Plus, TokenKind.Minus, TokenKind.Asterisk, TokenKind.ForwardSlash,
        TokenKind.Percent, TokenKind.Power, TokenKind.AND, TokenKind.OR, TokenKind.XOR, TokenKind.IsEqual,
        TokenKind.NotEqual, TokenKind.AssignmentOperator, TokenKind.LogicalAND, TokenKind.LogicalOR,
        TokenKind.LogicalXOR, TokenKind.NullishCoalescing, TokenKind.Increment, TokenKind.Decrement,
        TokenKind.ExclamationMark,
        };

    private static readonly TokenKind[] unaries = {
        TokenKind.Plus, TokenKind.Minus, TokenKind.Increment, TokenKind.Decrement, TokenKind.ExclamationMark
    };
    private static readonly TokenKind[] binaries = {
        TokenKind.Equal, TokenKind.Plus, TokenKind.Minus, TokenKind.Asterisk, TokenKind.ForwardSlash,
        TokenKind.Percent, TokenKind.Power, TokenKind.AND, TokenKind.OR, TokenKind.XOR, TokenKind.IsEqual,
        TokenKind.NotEqual, TokenKind.AssignmentOperator, TokenKind.LogicalAND, TokenKind.LogicalOR,
        TokenKind.LogicalXOR, TokenKind.NullishCoalescing,
    };

    private static readonly TokenKind[] bitwise = { 
        TokenKind.AND, TokenKind.OR, TokenKind.XOR
     };

    private static readonly TokenKind[] logicals = {
        TokenKind.LogicalAND, TokenKind.LogicalOR, TokenKind.LogicalXOR,
        };

    private static readonly TokenKind[] additives = {
        TokenKind.Plus, TokenKind.Minus
        };

    private static readonly TokenKind[] multiplicatives = { 
        TokenKind.Asterisk, TokenKind.ForwardSlash, TokenKind.Percent, TokenKind.Power
        };

    private static readonly TokenKind[] assignmentOperators = {
        TokenKind.Equal, TokenKind.AssignmentOperator
        };

    private static readonly TokenKind[] ignorables = {
        TokenKind.WhiteSpace, TokenKind.Comment, TokenKind.Unknown
        };

    // public static bool Is (this TokenKind kind) => .Contains(kind);
    public static bool IsLiteral(this TokenKind kind)         => literals.Contains(kind);
    public static bool IsUnaryOperator(this TokenKind kind)   => unaries.Contains(kind);
    public static bool IsBinaryOperator(this TokenKind kind)  => binaries.Contains(kind);
    public static bool IsOperator(this TokenKind kind)        => operators.Contains(kind);
    public static bool IsBitwise(this TokenKind kind)         => bitwise.Contains(kind);
    public static bool IsBoolOp(this TokenKind kind)         => logicals.Contains(kind);
    public static bool IsAdditive(this TokenKind kind)        => additives.Contains(kind);
    public static bool IsMultiplicative(this TokenKind kind)  => multiplicatives.Contains(kind);
    public static bool IsAssignment(this TokenKind kind)      => assignmentOperators.Contains(kind);
    public static bool IsParserIgnorable(this TokenKind kind) => ignorables.Contains(kind);

    public static int UnaryPrecedence(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Plus or
            TokenKind.Minus or
            TokenKind.Increment or
            TokenKind.Decrement or
            TokenKind.ExclamationMark => 4,

            _ => 0,
        };
    }

    public static int BinaryPrecedence(this TokenKind kind)
    {
        if (kind.IsBoolOp() || kind.IsBitwise())
            return 1;
        if (kind.IsAdditive())
            return 2;
        if (kind.IsMultiplicative())
            return 3;

        return 0;
    }
}