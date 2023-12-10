using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public abstract class LiteralValue
{
    public abstract object     Value { get; }
    public abstract TypeSymbol Type  { get; }

    public bool IsKnown   => Type.ID is not TypeID.Unknown;
    public bool IsDefined => this is not UndefinedValue;
    public bool IsGeneric => Type is GenericTypeSymbol;

    public abstract override string ToString();
    public abstract override bool   Equals(object? obj);
    public abstract override int    GetHashCode();

    public abstract string GetString();
}
