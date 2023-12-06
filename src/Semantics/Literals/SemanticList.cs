using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticList : SemanticExpression
{
    public SemanticExpression[] Elements    { get; }
    public TypeSymbol           ElementType { get; }

    public override Span         Span       { get; }
    public override TypeSymbol   Type       { get; }
    public override SemanticKind Kind  => SemanticKind.List;

    public SemanticList(SemanticExpression[] elements, TypeSymbol elementType, Span span)
    {
        Elements    = elements;
        ElementType = elementType;
        Type        = GenericTypeSymbol.List(elementType); 
        Span        = span;
    }
}
