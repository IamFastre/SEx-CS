
using SEx.Generic.Constants;
using SEx.Scoping;

namespace SEx.Evaluate.Values;

internal sealed class ListValue
    : GenericValue,
      IIterableValue<IntegerValue, LiteralValue>,
      IIterableValue<RangeValue, ListValue>
{

    private readonly List<LiteralValue> _values;
    public  override ValType[] ElementTypes { get; }

    public override object    Value => _values;
    public override ValType   Type  => ValType.List;

    public ValType      ElementType => ElementTypes[0];
    public IntegerValue Length      => new(_values.Count);


    public ListValue(IEnumerable<LiteralValue> list, ValType? type = null)
        : this (list.ToList(), type) { }

    public ListValue(List<LiteralValue> list, ValType? type = null)
    {
        _values      = list;
        ElementTypes = new ValType[1] { type ?? ValType.Any };
    }

    public static TypeSymbol GetIndexReturn(ValType index) => index switch
    {
        ValType.Integer => TypeSymbol.Any,
        ValType.Range   => GenericTypeSymbol.List(TypeSymbol.Any),
        _ => TypeSymbol.Unknown,
    };

    public override string ToString()
    {
        var str = C.GREEN2 + "[" + C.END;
        for (int i = 0; i < _values.Count; i++)
        {
            var val = _values[i].ToString();

            if (i != _values.Count - 1)
                str += $"{val}, ";
            else
                str += $"{val}";
        }
        str += $"{C.GREEN2}]{C.END}";

        return str;
    }

    public override string str()
    {
        var str = "[";
        for (int i = 0; i < _values.Count; i++)
        {
            var val = _values[i].str();

            if (i != _values.Count - 1)
                str += $"{val}, ";
            else
                str += $"{val}";
        }
        str += "]";

        return str;
    }

    public bool Contains(LiteralValue value)
        => _values.Contains(value);

    public LiteralValue? GetElement(IntegerValue index)
    {
        var i = double.IsNegative((double) index.Value)
                ? (_values.Count + ((double) index.Value))
                : ((double) index.Value);

        return i >= 0
            && _values.Count > i
            && int.TryParse(index.Value.ToString(), out _)
            ?  _values[(int)i]
            :  null;
    }

    public ListValue? GetElement(RangeValue index)
    {
            List<LiteralValue> slice;
            Range range = index.GetSystemRange();

            try { slice = _values.ToArray()[range].ToList(); }
            catch { return null; }

            var fin = index.GetSlice(slice.ToArray()).ToList();
            return new(fin, ElementType);
    }

    public ListValue Concat(ListValue other)
        => new(_values.Concat(other._values));
}
