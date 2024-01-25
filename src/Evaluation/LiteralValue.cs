using SEx.Scoping.Symbols;

namespace SEx.Evaluation.Values;

public abstract class LiteralValue
{
    public abstract object     Value { get; }
    public abstract TypeSymbol Type  { get; }

    public abstract override string ToString();
    public abstract override bool   Equals(object? obj);
    public abstract override int    GetHashCode();

    public static bool operator !=(LiteralValue left, LiteralValue right)
        => !left.Equals(right);

    public static bool operator ==(LiteralValue left, LiteralValue right)
        =>  left.Equals(right);

    public abstract string GetString();
}
