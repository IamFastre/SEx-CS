using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

internal class NullValue : LiteralValue
{
    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Null;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.NULL;
}

internal sealed class VoidValue : LiteralValue
{
    public static readonly VoidValue Template = new();

    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Void;

    public override string ToString()
        => str();

    public override string str()
        => "";
}

internal sealed class UnknownValue : LiteralValue
{
    public static readonly UnknownValue Template = new();

    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Unknown;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.UNKNOWN;
}

internal sealed class UndefinedValue : LiteralValue
{
    private TypeSymbol _type = TypeSymbol.Null;
    public static UndefinedValue New(TypeSymbol? type = null) => new() {_type = type ?? TypeSymbol.Null};

    public override object     Value => null!;
    public override TypeSymbol Type  => _type;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.UNDEFINED;
}
