using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticReturnStatement : SemanticStatement
{
    public SemanticExpression Expression { get; }
    public TypeSymbol         Type       => Expression.Type;

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.ReturnStatement;

    public SemanticReturnStatement(SemanticExpression expr, Span span)
    {
        Expression = expr;
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => new[] { Expression };
}