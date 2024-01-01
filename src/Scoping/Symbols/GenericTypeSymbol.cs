using SEx.Generic.Constants;

namespace SEx.Scoping.Symbols;

public sealed class GenericTypeSymbol : TypeSymbol
{
    public TypeSymbol[] Parameters { get; private set; }

    public override SymbolKind Kind => SymbolKind.GenericType;

    public GenericTypeSymbol(string name, TypeID typeKind = TypeID.Any, TypeSymbol? elementType = null, params TypeSymbol[] parameters)
        : base(name, typeKind, elementType)
        => Parameters = parameters;

    public void SetParameters(params TypeSymbol[] parameters)
        => Parameters = parameters;

    public override bool Matches(TypeSymbol other)
    {
        if (other is GenericTypeSymbol gen && ID.HasFlag(other.ID))
        {
            if (Parameters.Length != gen.Parameters.Length)
                return false;

            for (int i = 0; i < Parameters.Length; i++)
                if (!Parameters[i].Matches(gen.Parameters[i]))
                    return false;

            return true;
        }

        return false;
    }


    public override string ToString()
    {
        if (ID is TypeID.Function)
        {            
            var paramsStr = string.Empty;

            for (int i = 1; i < Parameters.Length; i++)
            {
                paramsStr += Parameters[i].ToString();
                if (i != Parameters.Length - 1)
                    paramsStr += ", ";
            }

            return $"({paramsStr}) -> {Parameters[0]}";
        }

        if (ID is TypeID.List)
            return $"{Parameters[0]}[]";

        var str = Name + "<";
        for (int i = 0; i < Parameters.Length; i++)
        {
            var p = Parameters[i];

            if (i == Parameters.Length - 1)
                str += p.Name;
            else
                str += p.Name + ", ";
        }

        return str + ">";
    }

    public static TypeSymbol GetTypeByString(string? type, TypeSymbol[] symbols) => type switch
    {
        CONSTS.LIST     => symbols.Length == 1 ? TypedList(symbols[0]) : Unknown,
        CONSTS.FUNCTION => Function(symbols),
        _ => Unknown
    };
}