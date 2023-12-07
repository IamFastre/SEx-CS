using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

internal sealed class RangeValue : LiteralValue, IIterableValue<IntegerValue, NumberValue>
{
    public override object Value => null!;
    public override TypeSymbol Type => TypeSymbol.Range;

    public NumberValue Start { get; }
    public NumberValue End   { get; }
    public NumberValue Step  { get; }

    public int Direction => double.IsPositive((double) Step.Value) ? 1 : -1;

    public IntegerValue? Length
    {
        get
        {
            var len = Math.Floor(((double) End.Value - (double) Start.Value) / (double) Step.Value) + 1D;
            len = double.IsNaN(len) ? 0 : len;
            return len >= 0 ? new(len) : null;
        }
    }

    public RangeValue(NumberValue start, NumberValue end, NumberValue step)
    {
        Start = start;
        End   = end;
        Step  = step;
    }

    public override string ToString()
        => $"{Start}:{End}:{Step}";

    public override bool Equals(object? obj)
    {
        if (obj is RangeValue rng)
            return ((double) Start.Value) == ((double) rng.Start.Value)
                && ((double) End.Value)   == ((double) rng.End.Value)
                && ((double) Step.Value)  == ((double) rng.Step.Value);

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Start, End, Step);

    public override string str()
        => $"{Start.str()}:{End.str()}:{Step.str()}";

    public NumberValue? GetElement(IntegerValue index)
    {
        var val = NumberValue.Get((double) index.Value * (double) Step.Value + (double) Start.Value);
        return Contains(val) ? val : null;
    }

    public bool Contains(LiteralValue value)
    {
        var bigger  = ((double) Start.Value) > ((double) End.Value)
                    ? ((double) Start.Value)
                    : ((double) End.Value);

        var smaller = ((double) Start.Value) < ((double) End.Value)
                    ? ((double) Start.Value)
                    : ((double) End.Value);

        return (smaller <= ((double) value.Value)) && (bigger >= ((double) value.Value));
    }

    public static TypeSymbol GetIndexReturn(TypeID index) => index switch
    {
        TypeID.Integer => TypeSymbol.Number,
        _ => TypeSymbol.Unknown,
    };

    public Range GetSystemRange()
    {
        if (Direction == 1)
            return (int)(double) Start.Value..(((int)(double) End.Value) + 1);
        else
            return (int)(double) End.Value..(((int)(double) Start.Value) + 1);
    }

    public T[] GetSlice<T>(T[] slice)
    {
        List<T> fin = new();
        for (int i = 0; i < GetSystemRange().End.Value; i++)
        {
            var step = (int)(i * Math.Abs((double) Step.Value));
            if (step >= slice.Length)
                break;
            fin.Add(slice[step]);
        }

        if (Direction == -1)
            fin.Reverse();

        return fin.ToArray();
    }
}
