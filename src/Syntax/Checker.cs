using SEx.Generic.Constants;

namespace SEx.Lex;

// Provides methods and definitions for checking various characters and symbols
public static class Checker
{
    // Some checks definitions
    public static char[] Separators = {',','.',';',':','?'};
    public static char[] Operators  = {'=','+','-','*','/','%','!','&','|','^'};

    public static char[] OpnDQuotes   = {'"','«','“'}; // '„'
    public static char[] ClsDQuotes   = {'"','»','”'}; // '“'
    public static char[] OpnSQuotes   = {'\'','‹'};
    public static char[] ClsSQuotes   = {'\'','›'};

    public static string[] Assignments = {"+=","-=","*=","/=","%=","&=","|=","^=","**=","&&=","||=","??="};
    public static string[] Booleans    = { CONSTS.TRUE, CONSTS.FALSE };
    public static string[] Keywords    = {
        CONSTS.IF, CONSTS.ELSE ,CONSTS.WHILE ,CONSTS.WHILE ,CONSTS.IMPORT ,CONSTS.EXPORT
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
}