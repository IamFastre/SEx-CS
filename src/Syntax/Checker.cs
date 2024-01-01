using SEx.Generic.Constants;

namespace SEx.Lex;

// Provides methods and definitions for checking various characters and symbols
public static class Checker
{
    // Some checks definitions
    public static readonly char[] Separators = {',','.',';',':','?'};
    public static readonly char[] Operators  = {'=','+','-','*','/','%','!','&','|','^'};

    public static readonly char[] OpnStrQuotes = {'"','«','“'}; // '„'
    public static readonly char[] ClsStrQuotes = {'"','»','”'}; // '“'
    public static readonly char[] OpnChrQuotes = {'\'','‹'};
    public static readonly char[] ClsChrQuotes = {'\'','›'};

    public const char UNDERSCORE = '_';
    public const char DOT        = '.';
    public const char DOLLAR     = '$';

    public static char GetOtherPair(char C)
    {
        // Double Quotation marks
        if (OpnStrQuotes.Contains(C))
            return ClsStrQuotes[Array.IndexOf(OpnStrQuotes, C)];
        if (ClsStrQuotes.Contains(C))
            return OpnStrQuotes[Array.IndexOf(ClsStrQuotes, C)];

        // Single Quotation marks
        if (OpnChrQuotes.Contains(C))
            return ClsChrQuotes[Array.IndexOf(OpnChrQuotes, C)];
        if (ClsChrQuotes.Contains(C))
            return OpnChrQuotes[Array.IndexOf(ClsChrQuotes, C)];

        throw new Exception($"Char \"{C}\" seems to not have a valid pair.");
    }

    public static TokenKind GetIdentifierKind(string value)
    => value switch
    {
        CONSTS.IN       => TokenKind.InOperator,
        CONSTS.DELETE   => TokenKind.Delete,
        CONSTS.NULL     => TokenKind.Null,
        CONSTS.IF       => TokenKind.If,
        CONSTS.ELSE     => TokenKind.Else,
        CONSTS.WHILE    => TokenKind.While,
        CONSTS.FOR      => TokenKind.For,
        CONSTS.BREAK    => TokenKind.Break,
        CONSTS.CONTINUE => TokenKind.Continue,
        CONSTS.RETURN   => TokenKind.Return,

        string when CONSTS.BOOLS.Contains(value) => TokenKind.Boolean,
        string when CONSTS.TYPES.Contains(value) => TokenKind.Type,

        _ => TokenKind.Identifier,
    };
}