using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticForStatement : SemanticStatement
{
    public Token               For      { get; }
    public VariableSymbol      Variable { get; }
    public SemanticExpression  Iterable { get; }
    public SemanticStatement   Body     { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.ForStatement;

    public SemanticForStatement(Token @for, VariableSymbol variable, SemanticExpression iterable, SemanticStatement body)
    {
        For      = @for;
        Variable = variable;
        Iterable = iterable;
        Body     = body;

        Span     = new(@for.Span, body.Span);
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Iterable;
        yield return Body;
    }
}
