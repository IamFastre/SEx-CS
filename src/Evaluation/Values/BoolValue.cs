using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public sealed class BoolValue : LiteralValue
{
    private readonly bool _value;
    public override object Value => _value;
    public override TypeSymbol Type => TypeSymbol.Boolean;

    public BoolValue(bool value) => _value = value;

    public int ToInt()
        => _value ? 1 : 0;

    public override string ToString()
        => C.VIOLET + str() + C.END;

    public override bool Equals(object? obj)
    {
        if (obj is BoolValue @bool)
            return _value == @bool._value;

        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string str()
        => _value ? CONSTS.TRUE : CONSTS.FALSE;

}
