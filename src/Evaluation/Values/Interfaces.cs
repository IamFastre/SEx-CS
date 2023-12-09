using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public interface IIterableValue
{
    public abstract IntegerValue? Length { get; }
    public static LiteralValue? GetElement(LiteralValue iterator, LiteralValue index)
    {
        switch (iterator)
        {
            case StringValue str:
                if (index is IntegerValue intv1)
                    return str.GetElement(intv1);
                if (index is RangeValue rngv)
                    return str.GetElement(rngv);
                break;

            case RangeValue rng:
                if (index is IntegerValue intv2)
                    return rng.GetElement(intv2);
                break;

            case ListValue lst:
                if (index is IntegerValue intv3)
                    return lst.GetElement(intv3);
                if (index is RangeValue rngv2)
                    return lst.GetElement(rngv2);
                break;
        }

        return UnknownValue.Template;
    }
}

public interface IIterableValue<Input, Output> : IIterableValue
    where Input  : LiteralValue
    where Output : LiteralValue
{
    public Output? GetElement(Input index);
    public bool    Contains(LiteralValue value);
}
