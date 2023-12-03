namespace SEx.Evaluate.Values;

internal abstract class LiteralValue
{
    public abstract object Value { get; }
    public abstract ValType Type { get; }

    public bool IsKnown   => this is not UnknownValue;
    public bool IsDefined => this is not UndefinedValue;
    public bool IsGeneric => this is     GenericValue;

    public ValType GetValueType() => this switch
    {
        VoidValue    => ValType.Void,
        UnknownValue => ValType.Unknown,
        NullValue    => ValType.Null,
        BoolValue    => ValType.Boolean,
        IntegerValue => ValType.Integer,
        FloatValue   => ValType.Float,
        CharValue    => ValType.Char,
        StringValue  => ValType.String,
        RangeValue   => ValType.Range,
        ListValue    => ValType.List,
        NumberValue  => ValType.Number,
        _            => ValType.Any,
    };

    public abstract override string ToString();
    public abstract string str();
}
