namespace SEx.Scoping.Symbols;

public static class BuiltIn
{
    public static readonly Dictionary<string, FunctionSymbol> Functions = new()
    {
        { "log", new("log", TypeSymbol.Void, new ParameterSymbol("value", TypeSymbol.String)) }
    };
}