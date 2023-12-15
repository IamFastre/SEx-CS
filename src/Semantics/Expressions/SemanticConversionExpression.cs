using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal enum ConversionKind
{
    AnyToString,
    Direct,

    NumberToInt,
    NumberToFloat,
    NumberToChar,

    IntToFloat,
    IntToChar,

    FloatToInt,
    FloatToChar,

    CharToInt,
    CharToFloat,

    RangeToNumberList,
    RangeToIntList,
    RangeToFloatList,

    StringToCharList,
    StringToStringList,
}

internal class SemanticConversionExpression : SemanticExpression
{
    public SemanticExpression Expression     { get; }
    public TypeSymbol         Destination    { get; }
    public ConversionKind     ConversionKind { get; }

    public override Span         Span        { get; }
    public override SemanticKind Kind => SemanticKind.ConversionExpression;

    public SemanticConversionExpression(SemanticExpression expr, TypeSymbol dest, ConversionKind cvKind, Span span)
        : base(dest)
    {
        Expression     = expr;
        Destination    = dest;
        ConversionKind = cvKind;

        Span           = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Expression;
    }

    public static ConversionKind? GetConversionKind(TypeSymbol from, TypeSymbol to)
    {
        if (to.Matches(from))
            return ConversionKind.Direct;

        return (from.ID, to.ID) switch
        {
            ( _ , TypeID.String) when from.IsKnown => ConversionKind.AnyToString,

            (TypeID.Number, TypeID.Integer)        => ConversionKind.NumberToInt,
            (TypeID.Number, TypeID.Float)          => ConversionKind.NumberToFloat,
            (TypeID.Number, TypeID.Char)           => ConversionKind.NumberToChar,

            (TypeID.Integer, TypeID.Char)          => ConversionKind.IntToChar,
            (TypeID.Integer, TypeID.Float)         => ConversionKind.IntToFloat,

            (TypeID.Float, TypeID.Integer)         => ConversionKind.FloatToInt,
            (TypeID.Float, TypeID.Char)            => ConversionKind.FloatToChar,

            (TypeID.Char, TypeID.Integer)          => ConversionKind.CharToInt,
            (TypeID.Char, TypeID.Float)            => ConversionKind.CharToFloat,

            (TypeID.Range, TypeID.List)            => to.ElementType!.ID is TypeID.Number
                                                    ? ConversionKind.RangeToNumberList
                                                    : to.ElementType!.ID is TypeID.Integer
                                                    ? ConversionKind.RangeToIntList
                                                    : to.ElementType!.ID is TypeID.Float
                                                    ? ConversionKind.RangeToFloatList
                                                    : null,

            (TypeID.String, TypeID.List)           => to.ElementType!.ID is TypeID.Char
                                                    ? ConversionKind.StringToCharList
                                                    : to.ElementType!.ID is TypeID.String
                                                    ? ConversionKind.StringToStringList
                                                    : null,
            _  => null,
        };
    }
}