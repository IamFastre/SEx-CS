using SEx.Lex;

namespace SEx.Generic.Constants;

internal static class CONSTS
{
    public const string EMPTY     = "";

    public const string UNKNOWN   = "?";
    public const string NULL      = "null";
    public const string VOID      = "void";

    public const string NAN       = "NaN";
    public const string NANF      = "NaNf";

    public const string TRUE      = "true";
    public const string MAYBE     = "maybe";
    public const string FALSE     = "false";

    public const string IF        = "if";
    public const string ELSE      = "else";
    public const string WHILE     = "while";
    public const string FOR       = "for";
    public const string RETURN    = "return";
    public const string BREAK     = "break";
    public const string CONTINUE  = "continue";

    public const string IMPORT    = "import";
    public const string EXPORT    = "export";
    public const string DELETE    = "delete";

    public const string IN        = "in";

    public const string BOOLEAN   = "bool";
    public const string INTEGER   = "int";
    public const string FLOAT     = "float";
    public const string NUMBER    = "number";
    public const string RANGE     = "range";
    public const string CHAR      = "char";
    public const string STRING    = "string";
    public const string LIST      = "list";
    public const string FUNCTION  = "function";
    public const string ACTION    = "action";
    public const string ANY       = "any";

    public static string[] BOOLS  = { TRUE, MAYBE, FALSE };
    public static string[] TYPES  = { VOID, BOOLEAN, INTEGER, FLOAT, NUMBER, CHAR, STRING, RANGE, LIST, FUNCTION, ACTION, ANY };

    public const string _SEX_     = "SEx";
    public const string _VERSION_ = "0.1.2-alpha";
}


