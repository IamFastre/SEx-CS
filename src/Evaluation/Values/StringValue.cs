using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Evaluate.Values;

internal sealed class StringValue
    : LiteralValue, IIterableValue<IntegerValue, CharValue>, IIterableValue<RangeValue, StringValue>
{
    private readonly string _value;
    public override object Value => _value;
    public override ValType Type => ValType.String;

    public StringValue(string value) => _value = value;

    public override string ToString()
        => $"{C.BLUE2}\"{_value.Escape()}\"{C.END}";

    public bool Contains(LiteralValue value)
        => _value.Contains(value.Value.ToString()!);

    public static ValType GetIndexReturn(ValType index) => index switch
    {
        ValType.Integer => ValType.Char,
        ValType.Range   => ValType.String,
        _ => ValType.Unknown,
    };

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

    public StringValue? GetElement(RangeValue index)
    {
            string slice;
            Range range;
            if (index.Direction == 1)
                range = (int)(double) index.Start.Value..(((int)(double) index.End.Value) + 1);
            else
                range = (int)(double) index.End.Value..(((int)(double) index.Start.Value) + 1);

            try { slice = _value[range]; }
            catch { return null; }

            string _ = string.Empty;
            for (int i = 0; i < range.End.Value; i++)
            {
                var step = (int)(i * Math.Abs((double) index.Step.Value) + range.Start.Value);
                if (step > range.End.Value)
                    break;
                _ += slice[step];
            }

            return new(index.Direction == 1 ? _ : new(_.Reverse().ToArray()));
    }
}
