using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticFormatString : SemanticExpression
{
    public SemanticNode[] Nodes { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.FormatString;

    public SemanticFormatString(SemanticNode[] exprs, Span span)
        : base(TypeSymbol.String)
    {
        Nodes = exprs;
        Span  = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Nodes;
}
