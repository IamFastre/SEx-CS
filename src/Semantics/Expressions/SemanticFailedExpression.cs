using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticFailedExpression : SemanticExpression
{
    public override Span Span { get; }

    public override SemanticKind Kind => SemanticKind.FailedExpression;

    public SemanticFailedExpression(Span span)
        : base(TypeSymbol.Unknown)
        => Span = span;

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}