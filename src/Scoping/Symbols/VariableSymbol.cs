using SEx.Evaluate.Values;

namespace SEx.Scoping;

internal sealed class VariableSymbol
{
    public string    Name       { get; }
    public ValueType Type       { get; }
    public bool      IsConstant { get; }

    public VariableSymbol(string name, ValueType? type = null, bool isConstant = false)
    {
        Name       = name;
        Type       = type ?? ValType.Any;
        IsConstant = isConstant;
    }
}
