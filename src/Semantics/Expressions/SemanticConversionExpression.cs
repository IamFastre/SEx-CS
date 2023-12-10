using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal enum ConversionKind
{
    AnyToString,
    Direct,
    IntToChar,
    CharToInt,
    IntToFloat,
    FloatToInt,
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

        if (TypeSymbol.String.Matches(to))
            return ConversionKind.AnyToString;

        switch (from.ID, to.ID)
        {
            case (TypeID.Integer, TypeID.Char):
                return ConversionKind.IntToChar;

            case (TypeID.Char, TypeID.Integer):
                return ConversionKind.CharToInt;

            case (TypeID.Integer, TypeID.Float):
                return ConversionKind.IntToFloat;

            case ( TypeID.Float, TypeID.Integer):
                return ConversionKind.FloatToInt;
        }

        return null;
    }
}