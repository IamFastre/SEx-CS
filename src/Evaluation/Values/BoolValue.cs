using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluation.Values;

public sealed class BoolValue : LiteralValue
{
    private readonly bool _value;
    public override object Value => _value;
    public override TypeSymbol Type => TypeSymbol.Boolean;

    public BoolValue(bool value) => _value = value;

    public int ToInt()
        => _value ? 1 : 0;

    public override string ToString()
        => C.MAGENTA + GetString() + C.END;

    public override bool Equals(object? obj) => obj is BoolValue bol && _value == bol._value; 
    public override int GetHashCode()        => _value.GetHashCode();

    public override string GetString()
        => _value ? CONSTS.TRUE : CONSTS.FALSE;

}
