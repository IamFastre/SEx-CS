using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public sealed class SemanticForStatement : SemanticStatement
{
    public VariableSymbol      Variable { get; }
    public SemanticExpression  Iterable { get; }
    public SemanticStatement   Body     { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.ForStatement;

    public SemanticForStatement(VariableSymbol variable, SemanticExpression iterable, SemanticStatement body, Span span)
    {
        Variable = variable;
        Iterable = iterable;
        Body     = body;

        Span     = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Iterable;
        yield return Body;
    }
}
