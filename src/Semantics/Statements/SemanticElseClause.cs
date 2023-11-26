using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticElseClause : SemanticClause
{
    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.ElseClause;

    public Token             Else  { get; }
    public SemanticStatement Body  { get; }

    public SemanticElseClause(Token @else, SemanticStatement body)
    {
        Else = @else;
        Body = body;
        Span = new(@else.Span, body.Span);
    }
}
