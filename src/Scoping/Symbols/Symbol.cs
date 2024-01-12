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
