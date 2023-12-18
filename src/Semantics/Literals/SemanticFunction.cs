using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public class SemanticFunction : SemanticExpression
{
    public FunctionSymbol        Symbol { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.Function;

    public SemanticFunction(FunctionSymbol symbol, Span span)
        : base(symbol.Type)
    {
        Symbol = symbol;
        Span   = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}
