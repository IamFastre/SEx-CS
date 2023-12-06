using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

internal sealed class BoolValue : LiteralValue
{
    private readonly bool _value;
    public override object Value => _value;
    public override TypeSymbol Type => TypeSymbol.Boolean;

    public BoolValue(bool value) => _value = value;

    public int ToInt()
        => _value ? 1 : 0;

    public override string ToString()
        => C.VIOLET + str() + C.END;
    
    public override string str()
        => _value ? CONSTS.TRUE : CONSTS.FALSE;
}
