
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

internal interface IIterableValue
{
    public ValType GetIterType(ValType type);
}

internal interface IIterableValue<Input, Output> : IIterableValue
    where Input  : LiteralValue
    where Output : LiteralValue
{
    public Output? Iterate(Input index);
    public bool    Contains(LiteralValue value);
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

internal abstract class NumberValue : LiteralValue
{
    public override ValType Type => ValType.Number;
    public abstract string SimpleString();
}

internal sealed class IntegerValue : NumberValue
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

    public override string ToString()
        => C.YELLOW2 + _value.ToString().Replace('E', 'e') + C.END;

    public override string SimpleString()
        => ToString();
}

internal sealed class FloatValue : NumberValue
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

    public override string SimpleString()
        => ToString().Replace("f", "");
}

internal sealed class CharValue : LiteralValue
{

    private readonly char _value;
    public override object Value => _value;
    public override ValType Type => ValType.Char;

    public CharValue(char value) => _value = value;

    public override string ToString() => $"{C.BLUE2}'{_value.ToString().Escape()}'{C.END}";
}

internal sealed class StringValue : LiteralValue, IIterableValue<IntegerValue, CharValue>
{
    private readonly string _value;
    public override object Value => _value;
    public override ValType Type => ValType.String;

    public StringValue(string value) => _value = value;

    public override string ToString() => $"{C.BLUE2}\"{_value.Escape()}\"{C.END}";
    public ValType GetIterType(ValType type) => ValType.Char;

    public CharValue? Iterate(IntegerValue index)
        =>  (double) index.Value >= 0
         && _value.Length > (double) index.Value
         && int.TryParse(index.Value.ToString(), out _)
         ?  new(_value[(int)(double) index.Value])
         :  null;

    public bool Contains(LiteralValue value)
        => _value.Contains(value.Value.ToString()!);
}

internal sealed class RangeValue : LiteralValue, IIterableValue<IntegerValue, IntegerValue>
{
    public override object Value => null!;
    public override ValType Type => ValType.Range;

    public NumberValue Start { get; }
    public NumberValue End   { get; }
    public NumberValue Step  { get; }

    public IntegerValue? Length
    {
        get
        {
            var len = new IntegerValue(
                Math.Floor(((double) End.Value - (double) Start.Value) / (double) Step.Value) + 1D
            );
            return double.IsPositive((double) len.Value) ? len : null;
        }
    }

    public RangeValue(NumberValue start, NumberValue end, NumberValue step)
    {
        Start = start;
        End   = end;
        Step  = step;
    }

    public override string ToString() => $"{Start.SimpleString()}:{End.SimpleString()}:{Step.SimpleString()}";
    public ValType GetIterType(ValType type) => ValType.Integer;

    public IntegerValue? Iterate(IntegerValue index)
    {
        var val = new IntegerValue((double) index.Value * (double) Step.Value + (double) Start.Value);
        return ((double) Start.Value) <= ((double) val.Value) &&
               ((double) val.Value)   <= ((double) End.Value)
               ? val : null;
    }

    public bool Contains(LiteralValue value)
        => ((double) Start.Value) <= ((double) value.Value) &&
           ((double) value.Value) <= ((double) End.Value);
}
