using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

internal sealed class CharValue : LiteralValue
{

    private readonly char _value;
    public  override object Value => _value;
    public  override TypeSymbol Type => TypeSymbol.Char;

    public CharValue(char value) => _value = value;

    public override string ToString()
    => $"{C.BLUE2}'{str()}'{C.END}";

    public override string str()
        => _value.ToString().Escape();
}
