namespace SEx.Evaluate.Values;

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
