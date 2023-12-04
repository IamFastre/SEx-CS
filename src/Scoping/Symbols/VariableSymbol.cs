using SEx.Evaluate.Values;

namespace SEx.Scoping;

internal class VariableSymbol : Symbol
{
    public ValType Type       { get; protected set; }
    public bool    IsConstant { get; protected set; }

    public override SymbolKind Kind => SymbolKind.Variable;

    public VariableSymbol(string name, ValType? type = null, bool isConstant = false)
        : base(name)
    {
        Type       = type ?? ValType.Any;
        IsConstant = isConstant;
    }

    public void MakeConstant() => IsConstant = true;

    public override int  GetHashCode()       => Name.GetHashCode();
    public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();

    public bool TestType(ValType type)
    {
        if ((Type, type).IsAssignable())
        {
            Type = type;
            return true;
        }

        return false;
    }
}
