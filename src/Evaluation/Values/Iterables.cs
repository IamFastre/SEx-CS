namespace SEx.Evaluate.Values;

internal interface IIterableValue
{
    public abstract static ValType GetIndexReturn(ValType index);

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
            
            case RangeValue str:
                if (index is IntegerValue intv2)
                    return str.GetElement(intv2);
                break;
        }

        return UnknownValue.Template;
    }
}

internal interface IIterableValue<Input, Output> : IIterableValue
    where Input  : LiteralValue
    where Output : LiteralValue
{
    public Output? GetElement(Input index);
    public bool    Contains(LiteralValue value);
}
