using SEx.Generic.Constants;

namespace SEx.Scoping.Symbols;

public class TypeSymbol : Symbol
{
    public TypeID      ID          { get; }
    public TypeSymbol? ElementType { get; }

    public bool IsKnown    => ID is not TypeID.Unknown;
    public bool IsIterable => ElementType is not null;

    public override SymbolKind Kind => SymbolKind.Type;

    protected TypeSymbol(string name, TypeID typeKind = TypeID.Any, TypeSymbol? elementType = null)
        : base(name)
    {
        ID          = typeKind;
        ElementType = elementType;
    }

    public override int  GetHashCode()       => ToString().GetHashCode();
    public override bool Equals(object? obj) => obj?.ToString() == ToString();


    public virtual bool Matches(TypeSymbol other)
        => ID.HasFlag(other.ID);


    public TypeSymbol? GetIndexReturn(TypeSymbol index)
        => GetIndexReturn(this, index);

    public static TypeSymbol? GetIndexReturn(TypeSymbol parent, TypeSymbol index)
    {
        if (parent.ElementType is null)
            return null;

        return index.ID switch
        {
            TypeID.Integer => parent.ElementType!,
            TypeID.Range   => parent,
            _ => Unknown,
        };
    }


    public static TypeSymbol? GetTypeByID(TypeID type) => type switch
    {
        TypeID.Any     => Any,
        TypeID.Boolean => Boolean,
        TypeID.Number  => Number,
        TypeID.Integer => Integer,
        TypeID.Float   => Float,
        TypeID.Char    => Char,
        TypeID.String  => String,
        TypeID.Range   => Range,

        _              => null,
    };

    public static TypeSymbol? GetTypeByString(string? type) => type switch
    {
        CONSTS.ANY     => Any,
        CONSTS.BOOLEAN => Boolean,
        CONSTS.NUMBER  => Number,
        CONSTS.INTEGER => Integer,
        CONSTS.FLOAT   => Float,
        CONSTS.CHAR    => Char,
        CONSTS.STRING  => String,
        CONSTS.RANGE   => Range,

        _              => null,
    };


    // Special types
    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN, TypeID.Unknown);
    public static readonly TypeSymbol Void    = new(CONSTS.VOID,    TypeID.Void);
    public static readonly TypeSymbol Nones   = new("nones",        TypeID.Nones);

    // Data types
    public static readonly TypeSymbol Any     = new(CONSTS.ANY,     TypeID.Any);
    public static readonly TypeSymbol Null    = new(CONSTS.NULL,    TypeID.Null);
    public static readonly TypeSymbol Boolean = new(CONSTS.BOOLEAN, TypeID.Boolean);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER,  TypeID.Number);
    public static readonly TypeSymbol Whole   = new(CONSTS.WHOLE,   TypeID.Whole);
    public static readonly TypeSymbol Integer = new(CONSTS.INTEGER, TypeID.Integer);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT,   TypeID.Float);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR,    TypeID.Char);
    public static readonly TypeSymbol String  = new(CONSTS.STRING,  TypeID.String, Char);
    public static readonly TypeSymbol Range   = new(CONSTS.RANGE,   TypeID.Range,  Number);

    public static readonly GenericTypeSymbol List = new(CONSTS.LIST, $"{Any}[]", TypeID.List, Any, Any);
}

public static class TypeExtension
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
            || TypeSymbol.Nones.Matches(types.hint)
            || TypeSymbol.Null.Matches(types.value);
    }
}
