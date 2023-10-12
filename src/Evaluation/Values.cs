
using SEx.Generic.Constants;

namespace SEx.Evaluator.Values;

internal abstract class LiteralValue
{
    public object? Value { get; }
    public abstract ValType Type { get; }
    public static object Parse(object value) { throw new Exception(); }
    public abstract override string ToString();
}

internal sealed class NullValue : LiteralValue
{

    public new byte? Value => null;
    public override ValType Type => ValType.Boolean;

    public static new byte? Parse(object value) => null;
    public override string ToString() => CONSTS.NULL;   
}

internal sealed class BoolValue : LiteralValue
{

    public new bool Value { get; }
    public override ValType Type => ValType.Boolean;

    public BoolValue(object value)
    {
        Value = Parse(value);
    }

    public static new bool Parse(object value)
    {
        if (value is string str)
            return str != "";

        if (value is char chr)
            return chr != '\0';

        if (value.ToString() != "0" || value.ToString() != "0.0")
            return true;

        throw new Exception("Value evaluated to non-boolean value");
    }

    public override string ToString()
    {
        var str = Value ? CONSTS.TRUE : CONSTS.FALSE;
        return str;   
    }
}

internal sealed class IntegerValue : LiteralValue
{

    public new double Value { get; }
    public override ValType Type => ValType.Integer;

    public IntegerValue(object value)
    {
        Value = Parse(value);
    }

    public static new double Parse(object value)
    {
        var num = double.Parse(value.ToString()!);
        if (!double.IsInteger(num))
            throw new Exception("Value evaluated to non-integer value");

        return num;
    }

    public override string ToString()
    {
        var str = Value.ToString().Replace('E', 'e');
        return str;   
    }
}

internal sealed class FloatValue : LiteralValue
{

    public new double Value { get; }
    public override ValType Type => ValType.Float;

    public FloatValue(object value)
    {
        Value = Parse(value);
    }

    public static new double Parse(object value)
    {
        var num = double.Parse(value.ToString()!);
        return num;
    }

    public override string ToString()
    {
        var str = Value.ToString().Replace('E', 'e');
        return str.Contains('.') ? str : str.Replace("e", ".0e");
    }
}

internal sealed class CharValue : LiteralValue
{

    public new char Value { get; }
    public override ValType Type => ValType.Char;

    public CharValue(object value)
    {
        Value = Parse(value);
    }

    public static new char Parse(object value)
    {
        var chr = char.Parse(value.ToString()!);
        return chr;
    }

    public override string ToString() => Value.ToString();   
}

internal sealed class StringValue : LiteralValue
{

    public new string Value { get; }
    public override ValType Type => ValType.String;

    public StringValue(object value)
    {
        Value = Parse(value);
    }

    public static new string Parse(object value)
    {
        var str = value.ToString() ?? throw new Exception($"Cannot make a string out of {value}");
        return str;
    }

    public override string ToString() => Value;   
}
