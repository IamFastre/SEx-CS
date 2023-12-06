using SEx.AST;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public VariableSymbol        Variable   { get; }
    public Span                  VarSpan    { get; }
    public DeclarationStatement  Node       { get; }
    public SemanticExpression?   Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(VariableSymbol var, Span span, DeclarationStatement node, SemanticExpression? expr)
    {
        Variable   = var;
        VarSpan    = span;
        Node       = node;
        Expression = expr;
        Span       = node.Span;
    }
}
