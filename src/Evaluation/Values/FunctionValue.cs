using SEx.Generic.Constants;
using SEx.Scoping.Symbols;
using SEx.Semantics;

namespace SEx.Evaluate.Values;

public class FunctionValue : LiteralValue
{
    public FunctionSymbol    Symbol { get; }
    public SemanticStatement Body   { get; }

    public override object Value => Symbol;
    public override TypeSymbol Type { get; }

    public FunctionValue(FunctionSymbol symbol, SemanticStatement body)
    {
        Symbol = symbol;
        Body   = body;
        Type   = symbol.Type;
    }

    public override string ToString()
        => $"{C.CYAN}{GetString()}{C.END}";

    public override bool Equals(object? obj) => obj is FunctionValue fv && GetHashCode() == fv.GetHashCode();
    public override int GetHashCode()        => HashCode.Combine(Symbol, Type);

    public override string GetString()
        => Symbol.ToString();
}

public class BuiltinFunctionValue : FunctionValue
{
    public BuiltinFunctionValue(FunctionSymbol symbol)
        : base(symbol, null!) { }
}