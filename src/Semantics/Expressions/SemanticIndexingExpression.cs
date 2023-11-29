using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticIndexingExpression : SemanticExpression
{
    public SemanticExpression Iterable { get; }
    public SemanticExpression Index    { get; }

    public override Span         Span  { get; }
    public override ValType      Type  { get; }
    public override SemanticKind Kind => SemanticKind.IndexingExpression;

    public SemanticIndexingExpression(SemanticExpression iterable, SemanticExpression index, ValType type, Span span)
    {
        Iterable = iterable;
        Index    = index;

        Type     = type;
        Span     = span;
    }

    public static ValType? GetElementType(ValType iterator, ValType index) => iterator switch
    {
        ValType.String => StringValue.GetIndexReturn(index),
        ValType.Range  => RangeValue.GetIndexReturn(index),
        ValType.List   => ListValue.GetIndexReturn(index),

        _ => null,
    };
}