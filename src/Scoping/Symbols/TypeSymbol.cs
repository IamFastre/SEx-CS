using SEx.Generic.Constants;

namespace SEx.Scoping.Symbols;

public class TypeSymbol : Symbol
{
    public TypeID      ID          { get; }
    public TypeSymbol? ElementType { get; }

    public bool IsKnown    => ID is not TypeID.Unknown;
    public bool IsVoid     => ID is TypeID.Void;
    public bool IsIterable => ElementType is not null;
    public bool IsGeneric  => this is GenericTypeSymbol;
    public bool IsCallable => ID is TypeID.Function;

    public override SymbolKind Kind => SymbolKind.Type;

    protected TypeSymbol(string name, TypeID typeKind = TypeID.Any, TypeSymbol? elementType = null)
        : base(name)
    {
        ID          = typeKind;
        ElementType = elementType;
    
        if (!IsGeneric)
            Types.Add(this);
    }

    public override int  GetHashCode()       => ToString().GetHashCode();
    public override bool Equals(object? obj) => obj?.ToString() == ToString();

    public static readonly List<TypeSymbol> Types = new();

    public static bool operator ==(TypeSymbol left, TypeSymbol right) =>  left.Equals(right);
    public static bool operator !=(TypeSymbol left, TypeSymbol right) => !left.Equals(right);

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


    public static TypeSymbol GetTypeByID(TypeID id)
    {
        foreach (var type in Types)
            if (type.ID == id)
                return type;

        return Unknown;
    }

    public static TypeSymbol GetTypeByString(string? str)
    {
        foreach (var type in Types)
            if (type.Name == str)
                return type;

        return Unknown;
    }


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

    public static GenericTypeSymbol TypedList(TypeID id)
    {
        var type = GetTypeByID(id);
        return new(CONSTS.LIST, $"{type}[]", TypeID.List, type, type);
    }

    public static GenericTypeSymbol TypedList(TypeSymbol type)
        => new(CONSTS.LIST, $"{type}[]", TypeID.List, type, type);


    public static GenericTypeSymbol Function(TypeSymbol[] parameters)
        => Function(parameters[0], parameters[1..]);

    public static GenericTypeSymbol Function(TypeSymbol type, params TypeSymbol[] parameters)
    {
        var allParams = new List<TypeSymbol> { type };
        allParams.AddRange(parameters);

        var paramsStr = string.Empty;
        foreach (var param in parameters)
        {
            paramsStr += param.ToString();

            if (param != parameters.Last())
                paramsStr += ", ";
        }

        return new(CONSTS.FUNCTION, $"({paramsStr}) -> {type}", TypeID.Function, null, allParams.ToArray());
    }
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
