using SEx.Evaluate.Values;
using SEx.Scoping.Symbols;
using SEx.Semantics;

namespace SEx.Evaluate.Conversions;

internal static class Converter
{
    public static LiteralValue Convert(ConversionKind kind, LiteralValue value, TypeSymbol to)
    {
        if (!value.IsKnown)
            return UnknownValue.Template;

        switch (kind)
        {
            case ConversionKind.Direct:
                return value;

            case ConversionKind.AnyToString:
                return new StringValue(value.GetString());

            case ConversionKind.IntToChar:
                return new CharValue((char)(double) value.Value);

            case ConversionKind.CharToInt:
                return new IntegerValue((char) value.Value);

            case ConversionKind.IntToFloat:
                return new FloatValue((double) value.Value);

            case ConversionKind.FloatToInt:
                var _flt = (FloatValue) value;
                return new IntegerValue(Math.Floor((double) _flt.Value));
        }

        return UnknownValue.Template;
    }
}