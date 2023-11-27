using SEx.Generic.Constants;

namespace SEx.Lex;

// Provides methods and definitions for checking various characters and symbols
public static class Checker
{
    // Some checks definitions
    public static readonly char[] Separators = {',','.',';',':','?'};
    public static readonly char[] Operators  = {'=','+','-','*','/','%','!','&','|','^'};

    public static readonly char[] OpnDQuotes = {'"','«','“'}; // '„'
    public static readonly char[] ClsDQuotes = {'"','»','”'}; // '“'
    public static readonly char[] OpnSQuotes = {'\'','‹'};
    public static readonly char[] ClsSQuotes = {'\'','›'};

    public static readonly string[] Booleans = { CONSTS.TRUE, CONSTS.FALSE };
    public static readonly string[] Types    =
    {
        CONSTS.BOOLEAN, CONSTS.INTEGER, CONSTS.FLOAT, CONSTS.CHAR, CONSTS.STRING, CONSTS.RANGE
    };
    public static readonly string[] Keywords =
    {
        CONSTS.IMPORT ,CONSTS.EXPORT
    };



    public static char GetOtherPair(char C)
    {
        // Double Quotation marks
        if (OpnDQuotes.Contains(C))
            return ClsDQuotes[Array.IndexOf(OpnDQuotes, C)];
        if (ClsDQuotes.Contains(C))
            return OpnDQuotes[Array.IndexOf(ClsDQuotes, C)];

        // Single Quotation marks
        if (OpnSQuotes.Contains(C))
            return ClsSQuotes[Array.IndexOf(OpnSQuotes, C)];
        if (ClsSQuotes.Contains(C))
            return OpnSQuotes[Array.IndexOf(ClsSQuotes, C)];

        throw new Exception($"Char \"{C}\" seems to not having a pair.");
    }

    public static TokenKind GetIdentifierKind(string value)
    => value switch
    {
        CONSTS.IN     => TokenKind.InOperator,
        CONSTS.DELETE => TokenKind.Delete,
        CONSTS.NULL   => TokenKind.Null,
        CONSTS.IF     => TokenKind.If,
        CONSTS.ELSE   => TokenKind.Else,
        CONSTS.WHILE  => TokenKind.While,
        CONSTS.FOR    => TokenKind.For,

        string when Booleans.Contains(value) => TokenKind.Boolean,
        string when Types.Contains(value)    => TokenKind.Type,
        string when Keywords.Contains(value) => TokenKind.Keyword,

        _ => TokenKind.Identifier,
    };
}