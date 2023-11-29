using SEx.Generic.Text;
using SEx.Evaluate.Values;

namespace SEx.Semantics;

internal sealed class SemanticList : SemanticExpression
{
    public SemanticExpression[] Elements    { get; }
    public ValType              ElementType { get; }

    public override Span         Span       { get; }
    public override ValType      Type  => ValType     .List;
    public override SemanticKind Kind  => SemanticKind.List;

    public SemanticList(SemanticExpression[] elements, ValType elementType, Span span)
    {
        Elements    = elements;
        ElementType = elementType;
        Span        = span;
    }
}
