
using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SEx.AST;
using SEx.Generic.Constants;
using SEx.Lex;
using SEx.Semantics;

namespace SEx.Symbols.Types;

internal enum VTypes
{
    Integer,
}

internal abstract class LiteralValue<T>
{
    public T? Value { get; }
    public abstract override string ToString();
    public static T Parse(object value) { throw new Exception(); }
}

internal sealed class BoolValue : LiteralValue<bool>
{

    public new bool Value { get; }

    public BoolValue(SemanticLiteral literal) : this(literal.Value) { }
    public BoolValue(Literal literal) : this(literal.Value!) { }
    public BoolValue(Token literal) : this(literal.Value!) { }

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

internal sealed class IntegerValue : LiteralValue<double>
{

    public new double Value { get; }

    public IntegerValue(SemanticLiteral literal) : this(literal.Value) { }
    public IntegerValue(Literal literal) : this(literal.Value!) { }
    public IntegerValue(Token literal) : this(literal.Value!) { }

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

internal sealed class FloatValue : LiteralValue<double>
{

    public new double Value { get; }

    public FloatValue(SemanticLiteral literal) : this(literal.Value) { }
    public FloatValue(Literal literal) : this(literal.Value!) { }
    public FloatValue(Token literal) : this(literal.Value!) { }

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

internal sealed class CharValue : LiteralValue<char>
{

    public new char Value { get; }

    public CharValue(SemanticLiteral literal) : this(literal.Value) { }
    public CharValue(Literal literal) : this(literal.Value!) { }
    public CharValue(Token literal) : this(literal.Value!) { }

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

internal sealed class StringValue : LiteralValue<string>
{

    public new string Value { get; }

    public StringValue(SemanticLiteral literal) : this(literal.Value) { }
    public StringValue(Literal literal) : this(literal.Value!) { }
    public StringValue(Token literal) : this(literal.Value!) { }

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
