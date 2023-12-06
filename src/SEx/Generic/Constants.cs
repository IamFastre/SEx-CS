
using SEx.Lex;

namespace SEx.Generic.Constants;

internal static class CONSTS
{
    public const string NULL      = "null";
    public const string UNKNOWN   = "?";
    public const string UNDEFINED = UNKNOWN;
    public const string VOID      = "";

    public const string TRUE      = "true";
    public const string FALSE     = "false";

    public const string IF        = "if";
    public const string ELSE      = "else";
    public const string WHILE     = "while";
    public const string FOR       = "for";

    public const string IMPORT    = "import";
    public const string EXPORT    = "export";
    public const string DELETE    = "delete";

    public const string IN        = "in";

    public const string BOOLEAN   = "bool";
    public const string INTEGER   = "int";
    public const string FLOAT     = "float";
    public const string NUMBER    = "number";
    public const string WHOLE     = "whole";
    public const string CHAR      = "char";
    public const string STRING    = "string";
    public const string RANGE     = "range";
    public const string LIST      = "list";
    public const string ANY       = "any";
    public static string[] TYPES  = { BOOLEAN, INTEGER, FLOAT, NUMBER, CHAR, STRING, RANGE, LIST, ANY };

    public const string _VERSION_ = "0.0.1";

}


