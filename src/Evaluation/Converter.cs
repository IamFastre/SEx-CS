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

            case ConversionKind.IntToFloat:
                return new FloatValue((double) value.Value);

            case ConversionKind.FloatToInt:
                return new IntegerValue(Math.Floor((double) value.Value));

            case ConversionKind.FloatToChar:
                return new CharValue((char) Math.Floor((double) value.Value));

            case ConversionKind.CharToInt:
                return new IntegerValue((char) value.Value);

            case ConversionKind.CharToFloat:
                return new FloatValue((char) value.Value);

            case ConversionKind.StringToCharList:
                List<CharValue> chars = new();
                foreach (var c in (string) value.Value)
                    chars.Add(new(c));

                return new ListValue(chars, to);

            case ConversionKind.StringToStringList:
                List<StringValue> strs = new();
                foreach (var c in (string) value.Value)
                    strs.Add(new(c.ToString()));

                return new ListValue(strs, to);
        }

        return UnknownValue.Template;
    }
}