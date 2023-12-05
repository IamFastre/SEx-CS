using SEx.Evaluate.Values;
using SEx.Generic.Constants;

namespace SEx.Scoping;

internal class TypeSymbol : Symbol
{
    public ValType     ID          { get; }
    public TypeSymbol? ElementType { get; }

    public bool IsKnown    => ID is not ValType.Unknown;
    public bool IsIterable => ElementType is not null;

    public override SymbolKind Kind => SymbolKind.Type;

    protected TypeSymbol(string name, ValType typeKind = ValType.Any, TypeSymbol? elementType = null)
        : base(name)
    {
        ID          = typeKind;
        ElementType = elementType;
    }

    public override int  GetHashCode()       => ToString().GetHashCode();
    public override bool Equals(object? obj) => obj?.ToString() == ToString();


    public virtual bool Matches(TypeSymbol other)
        => ID.HasFlag(other.ID);


    public static TypeSymbol GetNameType(string? type) => type switch
    {
        CONSTS.BOOLEAN => Boolean,
        CONSTS.NUMBER  => Number,
        CONSTS.INTEGER => Integer,
        CONSTS.FLOAT   => Float,
        CONSTS.CHAR    => Char,
        CONSTS.STRING  => String,
        CONSTS.RANGE   => Range,

        _              => Any,
    };


    // Special types
    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN, ValType.Unknown);
    public static readonly TypeSymbol Void    = new(CONSTS.VOID,    ValType.Void);
    public static readonly TypeSymbol Nones   = new("nones",        ValType.Nones);

    // Data types
    public static readonly TypeSymbol Any     = new(CONSTS.ANY,     ValType.Any);
    public static readonly TypeSymbol Null    = new(CONSTS.NULL,    ValType.Null);
    public static readonly TypeSymbol Boolean = new(CONSTS.BOOLEAN, ValType.Boolean);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER,  ValType.Number);
    public static readonly TypeSymbol Integer = new(CONSTS.INTEGER, ValType.Integer);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT,   ValType.Float);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR,    ValType.Char);
    public static readonly TypeSymbol String  = new(CONSTS.STRING,  ValType.String, Char);
    public static readonly TypeSymbol Range   = new(CONSTS.RANGE,   ValType.Range,  Number);
}

internal static class TypeExtension
{
    public static bool Match(this (TypeSymbol L, TypeSymbol R) Ns, TypeSymbol left, TypeSymbol? right = null, bool interchangeable = false)
    {
       if (right is null)
            return left.Matches(Ns.L) && left.Matches(Ns.R);

        if (interchangeable)
            return left.Matches(Ns.L) && right.Matches(Ns.R) || left.Matches(Ns.R) && right.Matches(Ns.L);
        return left.Matches(Ns.L) && right.Matches(Ns.R);
    }

    public static bool IsAssignable(this (TypeSymbol hint, TypeSymbol value) types, bool interchangeable = false)
    {
        if (interchangeable)
            return types.hint.Matches(types.value)
                || types.value.Matches(types.hint);

        return types.hint.Matches(types.value)
            || TypeSymbol.Nones.Matches(types.value)
            || TypeSymbol.Nones.Matches(types.hint);
    }
}