using SEx.AST;
using SEx.Generic.Text;
using SEx.Evaluate.Values;

namespace SEx.Semantics;

internal class SemanticName : SemanticExpression
{
    public string Value           { get; }
    public override Span    Span  { get; }
    public override ValType Type  { get; }
    public override SemanticKind Kind => SemanticKind.Name;

    public SemanticName(NameLiteral literal, ValType type)
    {
        Type  = type;
        Span  = literal.Span;
        Value = literal.Value;
    }
}
