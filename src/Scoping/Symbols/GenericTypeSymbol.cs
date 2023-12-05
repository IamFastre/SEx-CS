using SEx.Generic.Constants;

namespace SEx.Scoping;

internal sealed class GenericTypeSymbol : TypeSymbol
{
    public TypeSymbol[] Parameters { get; private set; }
    public override SymbolKind Kind => SymbolKind.GenericType;

    public GenericTypeSymbol(string name, params TypeSymbol[] parameters)
        : base(name) => Parameters = parameters;

    public void SetParameters(params TypeSymbol[] parameters)
        => Parameters = parameters;

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
        => new(CONSTS.LIST, type);
}