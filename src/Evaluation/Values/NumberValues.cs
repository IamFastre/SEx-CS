using SEx.Generic.Constants;

namespace SEx.Evaluate.Values;

internal abstract class NumberValue : LiteralValue
{
    public override ValType Type => ValType.Number;

    public static NumberValue Get(double value)
        => IntegerValue.IsEligible(value) ? new IntegerValue(value) : new FloatValue(value);
}

internal sealed class IntegerValue : NumberValue
{

    private readonly double _value;
    public  override object Value => _value;
    public  override ValType Type => ValType.Integer;

    public IntegerValue(double value)
    {
        if (!IsEligible(value))
            throw new Exception("Value given is not int");
        _value = value;
    }

    public static bool IsEligible(double value)
        => double.IsInteger(value) || double.IsInfinity(value)|| double.IsNaN(value);

    public override string ToString()
        => C.YELLOW2 + str() + C.END;

    public override string str()
        => _value.ToString().Replace('E', 'e');
}

internal sealed class FloatValue : NumberValue
{

    private readonly double _value;
    public  override object Value => _value;
    public  override ValType Type => ValType.Float;

    public FloatValue(double value) => _value = value;

    public override string ToString()
        => C.YELLOW2 + str() + C.END;

    public override string str()
    {
        var str = _value.ToString().Replace('E', 'e');
        if (!str.Contains('.')) str += ".0";

        return str + "f";
    }
}
