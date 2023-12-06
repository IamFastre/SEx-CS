using SEx.Generic.Constants;

namespace SEx.Scoping.Symbols;

internal sealed class GenericTypeSymbol : TypeSymbol
{
    public string?      CustomName { get; }
    public TypeSymbol[] Parameters { get; private set; }

    public override SymbolKind Kind => SymbolKind.GenericType;

    public GenericTypeSymbol(string name, string? customName = null, TypeID typeKind = TypeID.Any, TypeSymbol? elementType = null, params TypeSymbol[] parameters)
        : base(name, typeKind, elementType)
    {
        CustomName = customName;
        Parameters = parameters;
    }

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
        if (CustomName is not null)
            return CustomName;

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

    public static GenericTypeSymbol List(TypeSymbol type)
        => new(CONSTS.LIST, $"{type}[]", TypeID.List, type, type);
}