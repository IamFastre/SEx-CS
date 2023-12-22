using SEx.Scoping.Symbols;

namespace SEx.Scoping;

internal class SemanticScope
{
    public Dictionary<string, NameSymbol> Symbols { get; }
    public SemanticScope?                 Parent  { get; }

    public SemanticScope(SemanticScope? parent = null)
    {
        Symbols = new();
        Parent  = parent;

        if (parent is null)
            DeclareBuiltIns();
    }

    private void DeclareBuiltIns()
    {
        foreach (var func in BuiltIn.GetFunctions())
            TryDeclare(func.GetSymbol());
    }

    public bool IsDeclared(string name)
        => Symbols.ContainsKey(name);

    public bool TryDeclare(NameSymbol symbol)
    {
        if (IsDeclared(symbol.Name))
            return false;

        Symbols.Add(symbol.Name, symbol);
        return true;
    }

    public void Assign(NameSymbol name)
    {
        if (Symbols.ContainsKey(name.Name))
            Symbols[name.Name] = name;
    }

    public bool TryResolve(string name, out NameSymbol? symbol)
    {
        if (Symbols.TryGetValue(name, out var sym))
        {
            symbol = sym;
            return true;
        }

        symbol = Parent?.Resolve(name);
        return false;
    }

    public NameSymbol? Resolve(string name)
    {
        if (Symbols.TryGetValue(name, out var sym))
            return sym;
        
        return Parent?.Resolve(name);
    }
}
