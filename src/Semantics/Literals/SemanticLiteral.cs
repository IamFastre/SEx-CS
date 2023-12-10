using SEx.AST;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticLiteral : SemanticExpression
{
    public string Value                { get; }

    public override Span         Span  { get; }
    public override SemanticKind Kind => SemanticKind.Literal;

    public SemanticLiteral(Literal literal)
        : base(ToValueKind(literal.Kind))
    {
        Span  = literal.Span;
        Value = literal.Value;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}