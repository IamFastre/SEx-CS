using SEx.Evaluation.Values;
using SEx.Scoping.Symbols;
using SEx.SemanticAnalysis;

namespace SEx.Evaluation.Conversions;

internal static class Converter
{
    public static LiteralValue Convert(LiteralValue value, TypeSymbol to)
    {
        var kind = SemanticConversionOperation.GetConversionKind(value.Type, to);
        return kind is not null ? Convert(kind.Value, value, to) : UnknownValue.Template;
    }

    public static LiteralValue Convert(ConversionKind kind, LiteralValue value, TypeSymbol to)
    {
        if (!value.Type.IsKnown)
            return UnknownValue.Template;

        switch (kind)
        {
            case ConversionKind.Implicit:
                return value;

            case ConversionKind.Explicit:
                return Convert(value, to);

            case ConversionKind.AnyToString:
                return new StringValue(value.GetString());

            //==============Bools==============//

            case ConversionKind.BoolToNumber:
            case ConversionKind.BoolToInt:
                return new IntegerValue((bool) value.Value ? 1 : 0);

            case ConversionKind.BoolToFloat:
                return new FloatValue((bool) value.Value ? 1 : 0);

            //=============Numbers=============//

            case ConversionKind.NumberToInt:
            case ConversionKind.FloatToInt:
                return new IntegerValue((double) value.Value);

            case ConversionKind.NumberToFloat:
            case ConversionKind.IntToFloat:
                return new FloatValue((double) value.Value);

            case ConversionKind.NumberToChar:
            case ConversionKind.IntToChar:
            case ConversionKind.FloatToChar:
                return new CharValue((char) Math.Floor((double) value.Value));

            //==============Chars==============//

            case ConversionKind.CharToInt:
                return new IntegerValue((char) value.Value);

            case ConversionKind.CharToFloat:
                return new FloatValue((char) value.Value);

            //==============Lists==============//

            case ConversionKind.RangeToNumberList:
                return new ListValue(((RangeValue) value).GetEnumerator(), TypeSymbol.TypedList(TypeID.Number));

            case ConversionKind.RangeToIntList:
                return new ListValue(((RangeValue) value).GetEnumerator()
                                      .Select(i => new IntegerValue(Math.Floor((double) i.Value))), TypeSymbol.Integer);

            case ConversionKind.RangeToFloatList:
                return new ListValue(((RangeValue) value).GetEnumerator()
                                      .Select(i => new FloatValue((double) i.Value)), TypeSymbol.Float);

            case ConversionKind.StringToCharList:
                List<CharValue> chars = new();
                foreach (var c in (string) value.Value)
                    chars.Add(new(c));

                return new ListValue(chars, TypeSymbol.TypedList(TypeID.Char));

            case ConversionKind.StringToStringList:
                List<StringValue> strs = new();
                foreach (var c in (string) value.Value)
                    strs.Add(new(c.ToString()));

                return new ListValue(strs, TypeSymbol.TypedList(TypeID.String));
        }

        return UnknownValue.Template;
    }
}