using SEx.AST;
using SEx.Generic.Text;
using SEx.Evaluate.Values;
using SEx.Scoping;

namespace SEx.Semantics;

internal class SemanticVariable : SemanticExpression
{
    public VariableSymbol  Symbol { get; }
    public override Span    Span  { get; }
    public override ValType Type  { get; }

    public override SemanticKind Kind => SemanticKind.Name;

    public SemanticVariable(VariableSymbol symbol, Span span)
    {
        Symbol = symbol;
        Type   = symbol.Type;
        Span   = span;
    }
}
