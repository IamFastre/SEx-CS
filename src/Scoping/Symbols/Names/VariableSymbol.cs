namespace SEx.Scoping.Symbols;

public class VariableSymbol : NameSymbol
{
    public override SymbolKind Kind => SymbolKind.Variable;

    public VariableSymbol(string name, TypeSymbol? type = null, bool isConstant = false)
        : base(name, type, isConstant) { }
}
