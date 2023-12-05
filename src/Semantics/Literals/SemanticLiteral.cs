using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Scoping;

namespace SEx.Semantics;

internal sealed class SemanticLiteral : SemanticExpression
{
    public string Value              { get; }
    public override Span       Span  { get; }
    public override TypeSymbol Type  { get; }
    public override SemanticKind Kind => SemanticKind.Literal;

    public SemanticLiteral(Literal literal)
    {
        Type  = ToValueKind(literal.Kind);
        Span  = literal.Span;
        Value = literal.Value;
    }
}