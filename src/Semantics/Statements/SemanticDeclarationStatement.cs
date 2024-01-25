using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticDeclarationStatement : SemanticStatement
{
    public NameSymbol         Variable   { get; }
    public Span               VarSpan    { get; }
    public SemanticExpression Expression { get; }

    public override Span         Span    { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(NameSymbol var, Span varSpan, SemanticExpression expr, Span span)
    {
        Variable   = var;
        VarSpan    = varSpan;
        Expression = expr;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}
