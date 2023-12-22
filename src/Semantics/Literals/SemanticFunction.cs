using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticFunction : SemanticExpression
{
    public NameSymbol[]      Parameters { get; }
    public SemanticStatement Body       { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.Function;

    public SemanticFunction(NameSymbol[] parameters, SemanticStatement body, GenericTypeSymbol type, Span span)
        : base(type)
    {
        Parameters = parameters;
        Body       = body;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => new[] { Body };
}