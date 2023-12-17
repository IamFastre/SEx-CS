namespace SEx.Scoping.Symbols;

public class VariableSymbol : Symbol
{
    public TypeSymbol Type       { get; protected set; }
    public bool       IsConstant { get; protected set; }

    public override SymbolKind Kind => SymbolKind.Variable;

    public VariableSymbol(string name, TypeSymbol? type = null, bool isConstant = false)
        : base(name)
    {
        Type       = type ?? TypeSymbol.Any;
        IsConstant = isConstant;
    }

    public void MakeConstant() => IsConstant = true;

    public override int  GetHashCode()       => Name.GetHashCode();
    public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();

    public bool TestType(TypeSymbol type)
    {
        if ((Type, type).IsAssignable())
        {
            Type = type;
            return true;
        }

        return false;
    }
}
