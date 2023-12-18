using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public class SemanticName : SemanticExpression
{
    public NameSymbol        Symbol { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.Name;

    public SemanticName(NameSymbol symbol, Span span)
        : base(symbol.Type)
    {
        Symbol = symbol;
        Span   = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}
