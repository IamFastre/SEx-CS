using SEx.Generic.Constants;

namespace SEx.Evaluate.Values;

internal class NullValue : LiteralValue
{
    public override object Value => null!;
    public override ValType Type => ValType.Null;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.NULL;
}

internal sealed class VoidValue : LiteralValue
{
    public static readonly VoidValue Template = new();

    public override object Value => null!;
    public override ValType Type => ValType.Void;

    public override string ToString()
        => str();

    public override string str()
        => "";
}

internal sealed class UnknownValue : LiteralValue
{
    public static readonly UnknownValue Template = new();

    public override object Value => null!;
    public override ValType Type => ValType.Unknown;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.UNKNOWN;
}

internal sealed class UndefinedValue : LiteralValue
{
    private ValType _type;
    public static UndefinedValue New(ValType type = ValType.Null) => new() {_type = type};

    public override object Value => null!;
    public override ValType Type => _type;

    public override string ToString()
        => C.RED2 + str() + C.END;

    public override string str()
        => CONSTS.UNDEFINED;
}
