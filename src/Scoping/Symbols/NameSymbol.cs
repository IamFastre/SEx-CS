namespace SEx.Scoping.Symbols;

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

    public override int  GetHashCode()       => Name.GetHashCode();
    public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();
}