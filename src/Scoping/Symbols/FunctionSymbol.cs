namespace SEx.Scoping.Symbols;

public class FunctionSymbol : Symbol
{
    public TypeSymbol        Type       { get; }
    public TypeSymbol        ReturnType { get; }
    public ParameterSymbol[] Parameters { get; }

    public override SymbolKind Kind => SymbolKind.Function;

    public FunctionSymbol(string name, TypeSymbol type, params ParameterSymbol[] parameters)
        : base(name)
    {
        Type       = TypeSymbol.Function(type, parameters.Select(p => p.Type).ToArray());
        ReturnType = type;
        Parameters = parameters;
    }

    public override string ToString()
    {
        var str = $"{Name}(";

        for (int i = 0; i < Parameters.Length; i++)
        {
            var name = Parameters[i].ToString();
            var type = Parameters[i].Type.ToString();

            str += $"{name}:{type}";
            if (i != Parameters.Length - 1)
                str += ", ";
        }
        str += ")";

        return $"{str} -> {ReturnType}";
    }
}