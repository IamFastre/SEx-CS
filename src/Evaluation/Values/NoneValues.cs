using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public class NullValue : LiteralValue
{
    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Null;

    public override string ToString()
        => C.RED2 + GetString() + C.END;

    public override bool Equals(object? obj) => obj is NullValue;

    public override int GetHashCode() => 0;

    public override string GetString()
        => CONSTS.NULL;
}

public sealed class VoidValue : LiteralValue
{
    public static readonly VoidValue Template = new();

    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Void;

    public override string ToString()
        => GetString();

    public override bool Equals(object? obj) => obj is VoidValue;

    public override int GetHashCode() => 0;

    public override string GetString()
        => "";
}

public sealed class UnknownValue : LiteralValue
{
    public static readonly UnknownValue Template = new();

    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Unknown;

    public override string ToString()
        => C.RED2 + GetString() + C.END;

    public override bool Equals(object? obj) => obj is UnknownValue;

    public override int GetHashCode() => 0;

    public override string GetString()
        => CONSTS.UNKNOWN;
}

public sealed class UndefinedValue : LiteralValue
{
    private TypeSymbol _type = TypeSymbol.Null;
    public static UndefinedValue New(TypeSymbol? type = null) => new() {_type = type ?? TypeSymbol.Null};

    public override object     Value => null!;
    public override TypeSymbol Type  => _type;

    public override string ToString()
        => C.RED2 + GetString() + C.END;

    public override bool Equals(object? obj) => obj is UndefinedValue;

    public override int GetHashCode() => 0;

    public override string GetString()
        => CONSTS.UNDEFINED;
}
