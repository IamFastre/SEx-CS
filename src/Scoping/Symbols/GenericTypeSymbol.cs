using SEx.Evaluate.Values;
using SEx.Generic.Constants;

namespace SEx.Scoping;

internal sealed class GenericTypeSymbol : TypeSymbol
{
    public TypeSymbol[] Parameters { get; private set; }

    public override SymbolKind Kind => SymbolKind.GenericType;

    public GenericTypeSymbol(string name, ValType typeKind = ValType.Any, TypeSymbol? elementType = null, params TypeSymbol[] parameters)
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
                if (Parameters[i].ID != gen.Parameters[i].ID)
                    return false;

            return true;
        }

        return false;
    }


    public override string ToString()
    {
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
        => new(CONSTS.LIST, ValType.List, type, type);
}