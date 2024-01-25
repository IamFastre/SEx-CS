
using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluation.Values;

public sealed class ListValue
    : LiteralValue,
      IEnumerableValue<LiteralValue>,
      IIndexableValue<IntegerValue, LiteralValue>,
      IIndexableValue<RangeValue, ListValue>
{

    private readonly List<LiteralValue> _values;

    public override object Value => _values;
    public override GenericTypeSymbol Type { get; }

    public TypeSymbol   ElementType => Type.ElementType!;
    public IntegerValue Length      => new(_values.Count);

    public ListValue(IEnumerable<LiteralValue> list, TypeSymbol? type = null)
        : this (list.ToList(), type) { }

    public ListValue(List<LiteralValue> list, TypeSymbol? type = null)
    {
        _values = list;
        Type    = TypeSymbol.TypedList(type ?? TypeSymbol.Any);
    }

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

    public override bool Equals(object? obj)
    {
        if (obj is ListValue list)
        {
            if (_values.Count != list._values.Count)
                return false;
            
            for (int i = 0; i < _values.Count; i++)
                if (!_values[i].Equals(list._values[i]))
                    return false;
            return true;
        }
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_values);

    public override string GetString()
    {
        var str = "[";
        for (int i = 0; i < _values.Count; i++)
        {
            var val = _values[i].GetString();

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

    public LiteralValue? GetIndexed(IntegerValue index)
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

    public ListValue? GetIndexed(RangeValue index)
    {
            List<LiteralValue> slice;
            Range range = index.GetSystemRange();

            try { slice = _values.ToArray()[range].ToList(); }
            catch { return null; }

            var fin = index.GetSlice(slice.ToArray()).ToList();
            return new(fin, ElementType);
    }

    public ListValue Concat(ListValue other, TypeSymbol type)
        => new(_values.Concat(other._values), type);

    public IEnumerable<LiteralValue> GetEnumerator() => _values;

    public void TryModify(LiteralValue index, LiteralValue value)
    {
        if (index is RangeValue rng && value is ListValue lst) Modify(rng, lst);
        if (index is IntegerValue @int) Modify(@int, value);
    }

    public void Modify(RangeValue index, ListValue value)
    {
        int? first = null;
        foreach (var i in index.GetEnumerator())
            _values.RemoveAt(first ??= (int) Math.Floor((double) i.Value));
        
        if (first is not null)
            for (int i = 0; i < value._values.Count; i++)
                _values.Insert(first.Value + i, value._values[i]);
    }

    public void Modify(IntegerValue index, LiteralValue value)
        => _values[(int)(double) index.Value] = value;
}
