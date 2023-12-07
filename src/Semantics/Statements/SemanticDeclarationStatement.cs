using SEx.AST;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public VariableSymbol        Variable   { get; }
    public Span                  VarSpan    { get; }
    public SemanticExpression?   Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(VariableSymbol var, SemanticExpression? expr, DeclarationStatement node)
    {
        Variable   = var;
        VarSpan    = node.Variable.Span;
        Expression = expr;
        Span       = node.Span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}
