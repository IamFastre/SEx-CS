using SEx.AST;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticLiteral : SemanticExpression
{
    public string Value                { get; }

    public override Span         Span  { get; }
    public override SemanticKind Kind => SemanticKind.Literal;

    public SemanticLiteral(Literal literal)
        : base(ToValueType(literal.Kind))
    {
        Value = literal.Value;
        Span  = literal.Span;
    }

    public SemanticLiteral(string literal, TypeSymbol type, Span span)
        : base(type)
    {
        Value = literal;
        Span  = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Enumerable.Empty<SemanticNode>();
}