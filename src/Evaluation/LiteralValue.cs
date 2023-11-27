
using SEx.Generic;
using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Evaluate.Values;


internal abstract class LiteralValue
{
    public abstract object Value { get; }
    public abstract ValType Type { get; }

    public abstract override string ToString();
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

internal class NullValue : LiteralValue
{
    public override object Value => null!;
    public override ValType Type => ValType.Null;

    public override string ToString() => C.RED2 + CONSTS.NULL + C.END;
}

internal sealed class UndefinedValue : NullValue
{
    private ValType _type;
    public static UndefinedValue New(ValType type = ValType.Null) => new() {_type = type};

    public override ValType Type => _type;
}

internal sealed class BoolValue : LiteralValue
{
    private readonly bool _value;
    public override object Value => _value;
    public override ValType Type => ValType.Boolean;

    public BoolValue(bool value) => _value = value;

    public override string ToString() => C.VIOLET + (_value ? CONSTS.TRUE : CONSTS.FALSE) + C.END;
}

internal sealed class IntegerValue : LiteralValue
{

    private readonly double _value;
    public override object Value => _value;
    public override ValType Type => ValType.Integer;

    public IntegerValue(double value)
    {
        if (!(double.IsInteger(value) || double.IsInfinity(value)|| double.IsNaN(value)))
            throw new Exception("Value given is not int");
        _value = value;
    }

    public override string ToString() => C.YELLOW2 + _value.ToString().Replace('E', 'e') + C.END;
}

internal sealed class FloatValue : LiteralValue
{

    private readonly double _value;
    public override object Value => _value;
    public override ValType Type => ValType.Float;

    public FloatValue(double value) => _value = value;

    public override string ToString()
    {
        var str = _value.ToString().Replace('E', 'e');
        if (!str.Contains('.')) str += ".0";

        return C.YELLOW2 + str + "f" + C.END;
    }
}

internal sealed class CharValue : LiteralValue
{

    private readonly char _value;
    public override object Value => _value;
    public override ValType Type => ValType.Char;

    public CharValue(char value) => _value = value;

    public override string ToString() => $"{C.BLUE2}'{_value.ToString().Escape()}'{C.END}";
}

internal sealed class StringValue : LiteralValue
{

    private readonly string _value;
    public override object Value => _value;
    public override ValType Type => ValType.String;

    public StringValue(string value) => _value = value;

    public override string ToString() => $"{C.BLUE2}\"{_value.Escape()}\"{C.END}";
}

internal sealed class RangeValue : LiteralValue
{
    public override object Value => null!;
    public override ValType Type => ValType.Range;

    public IntegerValue Start { get; }
    public IntegerValue End   { get; }
    public IntegerValue Step  { get; }

    public RangeValue(IntegerValue start, IntegerValue end, IntegerValue step)
    {
        Start = start;
        End   = end;
        Step  = step;
    }

    public override string ToString() => $"{Start}:{End}:{Step}";
}
