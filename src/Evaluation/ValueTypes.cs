using SEx.Generic.Constants;

namespace SEx.Evaluate.Values;

[Flags]
internal enum ValType
{
    Void    = 0,
    Unknown = 1,
    Null    = 2,
    Boolean = 4,
    Integer = 8,
    Float   = 16,
    Char    = 32,
    String  = 64,

    Whole   = Integer | Char,
    Number  = Integer | Float,
    Numable = Number  | Char,
    Any     = Null | Boolean | Integer | Float | Char | String,
    Nones   = Void | Unknown | Null,
}

internal static class ValTypeExtension
{
    public static bool Match(this (ValType L, ValType R) Ns, ValType left, ValType? right = null, bool interchangeable = false)
    {
       if (right is null)
            return left.HasFlag(Ns.L) && left.HasFlag(Ns.R);

        if (interchangeable)
            return left.HasFlag(Ns.L) && right.Value.HasFlag(Ns.R) || left.HasFlag(Ns.R) && right.Value.HasFlag(Ns.L);
        return left.HasFlag(Ns.L) && right.Value.HasFlag(Ns.R);
    }

    public static bool Known(this (ValType L, ValType R) Ns)
        => Ns.L.Known() && Ns.R.Known();

    public static bool Known(this ValType type)
        => type is not ValType.Unknown or ValType.Void; 

    public static string ToString(this ValType type) => type.str();

    public static string str(this ValType type) => type switch
    {
        ValType.Unknown => CONSTS.UNKNOWN,
        ValType.Null    => CONSTS.NULL,
        ValType.Boolean => CONSTS.BOOLEAN,
        ValType.Integer => CONSTS.INTEGER,
        ValType.Float   => CONSTS.FLOAT,
        ValType.Char    => CONSTS.CHAR,
        ValType.String  => CONSTS.STRING,

        _ => type.ToString()
    };
}
