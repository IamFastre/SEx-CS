using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Evaluate.Values;

internal sealed class CharValue : LiteralValue
{

    private readonly char _value;
    public override object Value => _value;
    public override ValType Type => ValType.Char;

    public CharValue(char value) => _value = value;

    public override string ToString() => $"{C.BLUE2}'{_value.ToString().Escape()}'{C.END}";
}
