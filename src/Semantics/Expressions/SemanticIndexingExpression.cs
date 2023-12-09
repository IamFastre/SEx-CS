using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticIndexingExpression : SemanticExpression
{
    public SemanticExpression Iterable { get; }
    public SemanticExpression Index    { get; }

    public override Span         Span  { get; }
    public override TypeSymbol   Type  { get; }
    public override SemanticKind Kind => SemanticKind.IndexingExpression;

    public SemanticIndexingExpression(SemanticExpression iterable, SemanticExpression index, TypeSymbol type, Span span)
    {
        Iterable = iterable;
        Index    = index;

        Type     = type;
        Span     = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Iterable;
        yield return Index;
    }
}