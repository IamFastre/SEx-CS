using SEx.Lex;

namespace SEx.Evaluator.Values;

[Flags]
internal enum ValType
{
    Null    = 0,
    Boolean = 2,
    Integer = 4,
    Float   = 8,
    Char    = 16,
    String  = 32,

    Whole   = Integer | Char,
    Number  = Integer | Float | Char,
    Any     = Null | Boolean | Integer | Float | Char | String,
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
}