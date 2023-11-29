using SEx.Generic.Constants;

namespace SEx.Evaluate.Values;

internal class NullValue : LiteralValue
{
    public override object Value => null!;
    public override ValType Type => ValType.Null;

    public override string ToString() => C.RED2 + CONSTS.NULL + C.END;
}

internal sealed class VoidValue : LiteralValue
{
    public static readonly VoidValue Template = new();

    public override object Value => null!;
    public override ValType Type => ValType.Void;

    public override string ToString() => "";
}

internal sealed class UnknownValue : LiteralValue
{
    public static readonly UnknownValue Template = new();

    public override object Value => null!;
    public override ValType Type => ValType.Unknown;

    public override string ToString() => C.RED2 + CONSTS.UNKNOWN + C.END;
}

internal sealed class UndefinedValue : NullValue
{
    private ValType _type;
    public static UndefinedValue New(ValType type = ValType.Null) => new() {_type = type};

    public override ValType Type => _type;
}
