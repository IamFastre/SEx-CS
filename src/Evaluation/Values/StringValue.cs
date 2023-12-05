using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Scoping;

namespace SEx.Evaluate.Values;

internal sealed class StringValue
    : LiteralValue, IIterableValue<IntegerValue, CharValue>, IIterableValue<RangeValue, StringValue>
{
    private readonly string _value;
    public override object Value => _value;
    public override ValType Type => ValType.String;

    public IntegerValue Length => new(_value.Length);

    public StringValue(string value)
        => _value = value;

    public override string ToString()
        => $"{C.BLUE2}\"{str()}\"{C.END}";

    public override string str()
        => _value.Escape();

    public bool Contains(LiteralValue value)
        => _value.Contains(value.Value.ToString()!);

    public static TypeSymbol GetIndexReturn(ValType index) => index switch
    {
        ValType.Integer => TypeSymbol.Char,
        ValType.Range   => TypeSymbol.String,
        _               => TypeSymbol.Unknown,
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
            Range range = index.GetSystemRange();
  
            try { slice = _value[range]; }
            catch { return null; }

            string _ = new(index.GetSlice(slice.ToArray()));
            return new(_);
    }
}
