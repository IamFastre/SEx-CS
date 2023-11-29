using SEx.Generic.Constants;

namespace SEx.Evaluate.Values;

internal sealed class BoolValue : LiteralValue
{
    private readonly bool _value;
    public override object Value => _value;
    public override ValType Type => ValType.Boolean;

    public BoolValue(bool value) => _value = value;

    public override string ToString() => C.VIOLET + (_value ? CONSTS.TRUE : CONSTS.FALSE) + C.END;
}
