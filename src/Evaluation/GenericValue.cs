namespace SEx.Evaluate.Values;

internal abstract class GenericValue : LiteralValue
{
    public abstract ValType[] ElementTypes { get; }

    public virtual string TypeString
    {
        get
        {
            var _string = Type.str() + "<";

            for (int i = 0; i < ElementTypes.Length; i++)
            {
                var val = ElementTypes[i].str();

                if (i != ElementTypes.Length - 1)
                    _string += $"{val}, ";
                else
                    _string += $"{val}";
            }

            return _string + ">";
        }
    }

    public bool Matches(GenericValue other)
    {
        if (ElementTypes.Length != other.ElementTypes.Length)
            return false;
        
        for (int i = 0; i < ElementTypes.Length; i++)
            if (ElementTypes[i] != other.ElementTypes[i])
                return false;

        return true;
    }
}
