using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticLiteral : SemanticExpression
{
    public override SemanticKind Kind => SemanticKind.Literal;
    public override ValType Type  { get; }
    public override Span    Span  { get; }
    public string Value { get; }

    public SemanticLiteral(Literal literal)
    {
        Type  = ToValueKind(literal.Kind);
        Span  = literal.Span;
        Value = literal.Value;
    }
}