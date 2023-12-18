using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public sealed class StringValue
    : LiteralValue,
      IEnumerableValue<CharValue>,
      IIndexableValue<IntegerValue, CharValue>,
      IIndexableValue<RangeValue, StringValue>
{
    private readonly string _value;
    public override object Value => _value;
    public override TypeSymbol Type => TypeSymbol.String;

    public IntegerValue Length => new(_value.Length);

    public StringValue(string value)
        => _value = value;

    public override string ToString()
        => $"{C.BLUE2}\"{GetString()}\"{C.END}";

    public override bool Equals(object? obj) => obj is StringValue str && _value == str._value;
    public override int GetHashCode()        => _value.GetHashCode();

    public override string GetString()
        => _value.Escape();

    public bool Contains(StringValue value)
        => _value.Contains(value.Value.ToString()!);

    public bool Contains(CharValue value)
        => _value.Contains(value.Value.ToString()!);

    public static TypeSymbol GetIndexReturn(TypeID index) => index switch
    {
        TypeID.Integer => TypeSymbol.Char,
        TypeID.Range   => TypeSymbol.String,
        _               => TypeSymbol.Unknown,
    };

    public CharValue? GetIndexed(IntegerValue index)
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

    public StringValue? GetIndexed(RangeValue index)
    {
            string slice;
            Range range = index.GetSystemRange();
  
            try { slice = _value[range]; }
            catch { return null; }

            string _ = new(index.GetSlice(slice.ToArray()));
            return new(_);
    }

    public IEnumerable<CharValue> GetEnumerator()
    {
        foreach (var c in _value)
            yield return new(c);
    }
}
