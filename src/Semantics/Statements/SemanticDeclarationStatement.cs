using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticDeclarationStatement : SemanticStatement
{
    public NameSymbol         Variable   { get; }
    public SemanticExpression Expression { get; }

    public override Span         Span    { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(NameSymbol var, SemanticExpression expr, Span span)
    {
        Variable   = var;
        Expression = expr;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Expression;
    }
}
