using SEx.AST;
using SEx.Generic.Text;
using SEx.Evaluate.Values;

namespace SEx.Semantics;

internal class SemanticName : SemanticExpression
{
    public override SemanticKind Kind => SemanticKind.Name;
    public override ValType Type  { get; }
    public override Span    Span  { get; }
    public string Value { get; }

    public SemanticName(Name literal, ValType type)
    {
        Type  = type;
        Span  = literal.Span;
        Value = literal.Value;
    }
}