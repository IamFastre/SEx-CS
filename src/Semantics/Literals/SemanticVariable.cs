using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal class SemanticVariable : SemanticExpression
{
    public VariableSymbol      Symbol { get; }
    public override Span       Span   { get; }
    public override TypeSymbol Type   { get; }

    public override SemanticKind Kind => SemanticKind.Variable;

    public SemanticVariable(VariableSymbol symbol, Span span)
    {
        Symbol = symbol;
        Type   = symbol.Type;
        Span   = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}
