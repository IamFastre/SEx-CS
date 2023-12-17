namespace SEx.Scoping.Symbols;

public class ParameterSymbol : VariableSymbol
{
    public ParameterSymbol(string name, TypeSymbol? type = null)
        : base(name, type, false) { }
}