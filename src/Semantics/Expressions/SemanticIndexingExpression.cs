using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticIndexingExpression : SemanticExpression
{
    public SemanticExpression Iterable { get; }
    public SemanticExpression Index    { get; }

    public override Span         Span  { get; }
    public override SemanticKind Kind => SemanticKind.IndexingExpression;

    public SemanticIndexingExpression(SemanticExpression iterable, SemanticExpression index, TypeSymbol type, Span span)
        : base(type)
    {
        Iterable = iterable;
        Index    = index;

        Span     = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Iterable;
        yield return Index;
    }
}