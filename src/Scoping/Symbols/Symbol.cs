namespace SEx.Scoping.Symbols;

public enum SymbolKind
{
    Type,
    Variable,
    GenericType,
}

public abstract class Symbol
{
    public          string     Name    { get; }
    public abstract SymbolKind Kind    { get; }

    protected Symbol(string name) => Name = name;

    public override string ToString() => Name;
}

public sealed class NameSymbol : Symbol
{
    public TypeSymbol Type       { get; }
    public bool       IsConstant { get; private set; }

    public override SymbolKind Kind => SymbolKind.Variable;

    public NameSymbol(string name, TypeSymbol? type = null, bool isConstant = false)
        : base(name)
    {
        Type       = type ?? TypeSymbol.Any;
        IsConstant = isConstant;
    }

    public void MakeConstant()               => IsConstant = true;

    public override int  GetHashCode()       => ToString().GetHashCode();
    public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();
}