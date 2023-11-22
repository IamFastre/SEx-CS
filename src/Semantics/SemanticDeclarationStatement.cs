using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public Name                Name       { get; }
    public bool                IsConstant { get; }
    public SemanticExpression? Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(DeclarationStatement declaration, SemanticExpression? expression)
    {
        Name       = declaration.Name;
        IsConstant = declaration.IsConstant;
        Expression = expression;
        Span       = declaration.Span;
    }
}
