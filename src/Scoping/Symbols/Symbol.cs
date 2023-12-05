namespace SEx.Scoping;

internal enum SymbolKind
{
    Type,
    GenericType,
    Variable,
}

internal abstract class Symbol
{

    public string Name { get; }
    public abstract SymbolKind Kind { get; }

    protected Symbol(string name) => Name = name;

    public override string ToString() => Name;
}
