using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public class SemanticVariable : SemanticExpression
{
    public NameSymbol        Symbol { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.Variable;

    public SemanticVariable(NameSymbol symbol, Span span)
        : base(symbol.Type)
    {
        Symbol = symbol;
        Span   = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}
