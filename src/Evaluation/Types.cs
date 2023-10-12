using SEx.Evaluator.Values;

namespace SEx.Evaluator.Types;

internal enum SymbolTypes
{
    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String
}

internal static class SymbolTypesExtension
{
    public static LiteralValue Parse(this SymbolTypes type, object value) => type switch
    {
        SymbolTypes.Null    => new NullValue(),
        SymbolTypes.Boolean => new BoolValue(value),
        SymbolTypes.Integer => new IntegerValue(value),
        SymbolTypes.Float   => new FloatValue(value),
        SymbolTypes.Char    => new CharValue(value),
        SymbolTypes.String  => new StringValue(value),

        _ => throw new Exception("Unrecognized symbol type"),
    };
}