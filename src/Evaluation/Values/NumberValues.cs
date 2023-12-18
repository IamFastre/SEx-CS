using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public abstract class NumberValue : LiteralValue
{
    public override TypeSymbol Type => TypeSymbol.Number;

    public static NumberValue Get(double value)
        => IntegerValue.IsInt(value) ? new IntegerValue(value) : new FloatValue(value);

    public override bool Equals(object? obj)
    {
        if (obj is NumberValue num)
            return (double) Value == (double) num.Value;

        return false;
    }

    public override int GetHashCode() => ((double) Value).GetHashCode();

}

public sealed class IntegerValue : NumberValue
{
    private readonly double _value;
    public  override object Value => _value;
    public  override TypeSymbol Type => TypeSymbol.Integer;

    public IntegerValue(double value) => _value = Math.Floor(value);

    public static bool IsInt(double value)
        => double.IsInteger(value) || double.IsInfinity(value)|| double.IsNaN(value);

    public override string ToString()
        => C.YELLOW2 + GetString() + C.END;

    public override string GetString()
        => _value.ToString().Replace('E', 'e');
}

public sealed class FloatValue : NumberValue
{
    private readonly double _value;
    public  override object Value => _value;
    public  override TypeSymbol Type => TypeSymbol.Float;

    public FloatValue(double value) => _value = value;

    public override string ToString()
        => C.YELLOW2 + GetString() + C.END;

    public override string GetString()
        => _value.ToString().Replace('E', 'e') + "f";
}
