using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticWhileStatement : SemanticStatement
{
    public Token               While      { get; }
    public SemanticExpression  Condition  { get; }
    public SemanticStatement   Body       { get; }
    public SemanticElseClause? ElseClause { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.WhileStatement;

    public SemanticWhileStatement(Token @while, SemanticExpression condition, SemanticStatement body, SemanticElseClause? elseClause = null)
    {
        While      = @while;
        Condition  = condition;
        Body       = body;
        ElseClause = elseClause;

        Span = new(@while.Span, elseClause is not null
                              ? elseClause.Span
                              : body.Span);
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Condition;
        yield return Body;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
