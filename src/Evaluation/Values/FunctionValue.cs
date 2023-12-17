using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Evaluate.Values;

public sealed class FunctionValue : LiteralValue
{
    public FunctionSymbol Symbol { get; }
    public override object Value => null!;
    public override TypeSymbol Type { get; }

    public FunctionValue(FunctionSymbol symbol)
    {
        Symbol = symbol;
        Type   = symbol.Type;
    }

    public override string ToString()
        => $"{C.CYAN}{GetString()}{C.END}";

    public override bool Equals(object? obj) => obj is FunctionValue fv && GetHashCode() == fv.GetHashCode();
    public override int GetHashCode() => Type.GetHashCode();

    public override string GetString()
        => Symbol.ToString();
}