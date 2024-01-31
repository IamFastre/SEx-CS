using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticForStatement : SemanticStatement
{
    public NameSymbol          Variable { get; }
    public Span                VarSpan  { get; }
    public SemanticExpression  Iterable { get; }
    public SemanticStatement   Body     { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.ForStatement;

    public SemanticForStatement(NameSymbol variable, Span varSpan, SemanticExpression iterable, SemanticStatement body, Span span)
    {
        Variable = variable;
        VarSpan  = varSpan;
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
