namespace SEx.Evaluation.Values;

public interface IIndexableValue
{
    public abstract IntegerValue? Length { get; }

    public static LiteralValue? GetIndexed(LiteralValue iterator, LiteralValue index)
    {
        switch (iterator)
        {
            case StringValue str:
                if (index is IntegerValue intv1)
                    return str.GetIndexed(intv1);
                if (index is RangeValue rngv)
                    return str.GetIndexed(rngv);
                break;

            case RangeValue rng:
                if (index is IntegerValue intv2)
                    return rng.GetIndexed(intv2);
                break;

            case ListValue lst:
                if (index is IntegerValue intv3)
                    return lst.GetIndexed(intv3);
                if (index is RangeValue rngv2)
                    return lst.GetIndexed(rngv2);
                break;
        }

        return UnknownValue.Template;
    }
}

public interface IIndexableValue<Input, Output> : IIndexableValue
    where Output : LiteralValue
{
    public Output? GetIndexed(Input index);
}

public interface IEnumerableValue
{
    public static IEnumerable<LiteralValue>? GetIterator(LiteralValue iterator) => iterator switch
    {
        StringValue str => str.GetEnumerator(),
        RangeValue  rng => rng.GetEnumerator(),
        ListValue   lst => lst.GetEnumerator(),

        _ => null,
    };
}

public interface IEnumerableValue<Element> : IEnumerableValue
    where Element : LiteralValue
{
    public bool Contains(Element value);
    public IEnumerable<Element> GetEnumerator();
}
