using SEx.Generic.Constants;

namespace SEx.Lex;

// Provides methods and definitions for checking various characters and symbols
public static class Checker
{
    // Some checks definitions
    public static char[] Separators  = {',','.',';',':','?'};
    public static char[] Operators   = {'=','+','-','*','/','%','!','&','|','^'};

    public static char[] OpnQuotes   = {'"','«','„','“'};
    public static char[] ClsQuotes   = {'"','»','“','”'};

    public static string[] Assignments = {"+=","-=","*=","/=","%=","&=","|=","^=","**=","&&=","||=","??="};
    public static string[] Booleans    = { CONSTS.TRUE, CONSTS.FALSE };
    public static string[] Keywords    = {
        CONSTS.IF, CONSTS.ELSE ,CONSTS.WHILE ,CONSTS.WHILE ,CONSTS.IMPORT ,CONSTS.EXPORT
    };



    public static char GetOtherPair(char C)
    {
        // Quotation marks
        if (OpnQuotes.Contains(C))
            return ClsQuotes[Array.IndexOf(OpnQuotes, C)];
        if (ClsQuotes.Contains(C))
            return OpnQuotes[Array.IndexOf(ClsQuotes, C)];

        throw new Exception($"Char \"{C}\" seems to not having a pair.");
    }
}