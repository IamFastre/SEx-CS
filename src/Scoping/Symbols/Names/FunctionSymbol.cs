namespace SEx.Scoping.Symbols;

public class FunctionSymbol : NameSymbol
{
    public TypeSymbol        ReturnType { get; }
    public ParameterSymbol[] Parameters { get; }

    public override SymbolKind Kind => SymbolKind.Function;

    public FunctionSymbol(string name, TypeSymbol returnType, bool isConstant, params ParameterSymbol[] parameters)
        : base(name, TypeSymbol.Function(returnType, parameters.Select(p => p.Type).ToArray()), isConstant)
    {
        ReturnType = returnType;
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

    public override int  GetHashCode()       => HashCode.Combine(Name, Type, IsConstant, ReturnType, Parameters);
    public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();
}