
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
    public abstract static ValType GetIndexReturn(ValType index);

    public static LiteralValue? GetElement(LiteralValue iterator, LiteralValue index)
    {
        switch (iterator)
        {
            case StringValue str:
                if (index is IntegerValue intv1)
                    return str.GetElement(intv1);
                if (index is RangeValue rngv)
                    return str.GetElement(rngv);
                break;
            
            case RangeValue str:
                if (index is IntegerValue intv2)
                    return str.GetElement(intv2);
                break;
        }

        return null;
    }
}

internal interface IIterableValue<Input, Output> : IIterableValue
    where Input  : LiteralValue
    where Output : LiteralValue
{
    public Output? GetElement(Input index);
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

    public static NumberValue Get(double value)
        => IntegerValue.IsEligible(value) ? new IntegerValue(value) : new FloatValue(value);
}

internal sealed class IntegerValue : NumberValue
{

    private readonly double _value;
    public override object Value => _value;
    public override ValType Type => ValType.Integer;

    public IntegerValue(double value)
    {
        if (!IsEligible(value))
            throw new Exception("Value given is not int");
        _value = value;
    }

    public static bool IsEligible(double value)
        => double.IsInteger(value) || double.IsInfinity(value)|| double.IsNaN(value);

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

internal sealed class StringValue : LiteralValue, IIterableValue<IntegerValue, CharValue>, IIterableValue<RangeValue, StringValue>
{
    private readonly string _value;
    public override object Value => _value;
    public override ValType Type => ValType.String;

    public StringValue(string value) => _value = value;

    public override string ToString()
        => $"{C.BLUE2}\"{_value.Escape()}\"{C.END}";


    public CharValue? GetElement(IntegerValue index)
    {
        var i = double.IsNegative((double) index.Value)
                ? (_value.Length + ((double) index.Value))
                : ((double) index.Value);

        return i >= 0
            && _value.Length > i
            && int.TryParse(index.Value.ToString(), out _)
            ?  new (_value[(int)i])
            :  null;
    }

    public bool Contains(LiteralValue value)
        => _value.Contains(value.Value.ToString()!);

    public static ValType GetIndexReturn(ValType index) => index switch
    {
        ValType.Integer => ValType.Char,
        ValType.Range   => ValType.String,
        _ => ValType.Unknown,
    };

    public StringValue? GetElement(RangeValue index)
    {
            Range range;
            if (index.Direction == 1)
                range = (int)(double) index.Start.Value..(((int)(double) index.End.Value) + 1);
            else
                range = (int)(double) index.End.Value..(((int)(double) index.Start.Value) + 1);
            
            return new(_value[range]);
    }
}

internal sealed class RangeValue : LiteralValue, IIterableValue<IntegerValue, NumberValue>
{
    public override object Value => null!;
    public override ValType Type => ValType.Range;

    public NumberValue Start { get; }
    public NumberValue End   { get; }
    public NumberValue Step  { get; }

    public int Direction => double.IsPositive((double) Step.Value) ? 1 : -1;

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

    public override string ToString()
        => $"{Start.SimpleString()}:{End.SimpleString()}:{Step.SimpleString()}";

    public NumberValue? GetElement(IntegerValue index)
    {
        var val = NumberValue.Get((double) index.Value * (double) Step.Value + (double) Start.Value);
        return Contains(index) ? val : null;
    }

    public bool Contains(LiteralValue value)
    {
        var bigger  = ((double) Start.Value) > ((double) End.Value)
                    ? ((double) Start.Value)
                    : ((double) End.Value);

        var smaller = ((double) Start.Value) < ((double) End.Value)
                    ? ((double) Start.Value)
                    : ((double) End.Value);

        return smaller <= ((double) value.Value) &&
               bigger  >= ((double) value.Value);
    }

    public static ValType GetIndexReturn(ValType index) => index switch
    {
        ValType.Integer => ValType.Number,
        _ => ValType.Unknown,
    };
}
