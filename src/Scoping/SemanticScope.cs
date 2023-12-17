using SEx.Scoping.Symbols;

namespace SEx.Scoping;

internal class SemanticScope
{
    public Dictionary<string, Symbol> Symbols { get; }
    public SemanticScope? Parent { get; }

    public SemanticScope(SemanticScope? parent = null)
    {
        Symbols = new();
        Parent  = parent;

        if (parent is null)
            DeclareBuiltIns();
    }

    private void DeclareBuiltIns()
    {
        foreach (var func in FunctionSymbol.BuiltIns.Values)
            TryDeclare(func);
    }

    public VariableSymbol[] Variables => Symbols.Values.OfType<VariableSymbol>().ToArray();

    public bool IsDeclared(string name)
        => Symbols.ContainsKey(name);

    public bool TryDeclare(Symbol symbol)
    {
        if (IsDeclared(symbol.Name))
            return false;

        Symbols.Add(symbol.Name, symbol);
        return true;
    }

    public void Assign(VariableSymbol name)
    {
        if (Symbols.ContainsKey(name.Name))
            Symbols[name.Name] = name;
    }

    public bool TryResolve(string name, out Symbol? symbol)
    {
        if (Symbols.TryGetValue(name, out var sym))
        {
            symbol = sym;
            return true;
        }

        symbol = Parent?.Resolve(name);
        return false;
    }

    public Symbol? Resolve(string name)
    {
        if (Symbols.TryGetValue(name, out var sym))
            return sym;
        
        return Parent?.Resolve(name);
    }
}
