using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticForStatement : SemanticStatement
{
    public Token               For      { get; }
    public SemanticName        Variable { get; }
    public SemanticExpression  Iterable { get; }
    public SemanticStatement   Body     { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.ForStatement;

    public SemanticForStatement(Token @for, SemanticName variable, SemanticExpression iterable, SemanticStatement body)
    {
        For      = @for;
        Variable = variable;
        Iterable = iterable;
        Body     = body;

        Span     = new(@for.Span, body.Span);
    }
}
