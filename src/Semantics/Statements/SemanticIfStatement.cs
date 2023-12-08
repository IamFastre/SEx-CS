using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticIfStatement : SemanticStatement
{
    public Token               If         { get; }
    public SemanticExpression  Condition  { get; }
    public SemanticStatement   Then       { get; }
    public SemanticElseClause? ElseClause { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.IfStatement;

    public SemanticIfStatement(Token @if, SemanticExpression condition, SemanticStatement body, SemanticElseClause? elseClause = null)
    {
        If         = @if;
        Condition  = condition;
        Then       = body;
        ElseClause = elseClause;

        Span = new(@if.Span, elseClause is not null
                           ? elseClause.Span
                           : body.Span);
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Then;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
