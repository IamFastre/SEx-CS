using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public sealed class CharValue : LiteralValue
{

    private readonly char _value;
    public  override object Value => _value;
    public  override TypeSymbol Type => TypeSymbol.Char;

    public CharValue(char value) => _value = value;

    public override string ToString()
        => $"{C.BLUE2}'{GetString()}'{C.END}";

    public override bool Equals(object? obj) => obj is CharValue chr && _value == chr._value;
    public override int GetHashCode()        => _value.GetHashCode();

    public override string GetString()
        => _value.ToString().Escape();
}
