using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Semantics;

public sealed class SemanticLiteral : SemanticExpression
{
    public string Value                { get; }

    public override Span         Span  { get; }
    public override SemanticKind Kind => SemanticKind.Literal;

    public SemanticLiteral(Literal literal)
        : base(ToValueKind(literal.Kind))
    {
        Value = literal.Value;
        Span  = literal.Span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}