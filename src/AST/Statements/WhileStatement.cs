using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class WhileStatement : Statement
{
    public Token       While      { get; }
    public Expression  Condition  { get; }
    public Statement   Body       { get; }
    public ElseClause? ElseClause { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.WhileStatement;

    public WhileStatement(Token @while, Expression condition, Statement body, ElseClause? elseClause = null)
    {
        While      = @while;
        Condition  = condition;
        Body       = body;
        ElseClause = elseClause;

        Span = new(@while.Span, elseClause is not null
                              ? elseClause.Span
                              : body.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return While.Node;
        yield return Condition;
        yield return Body;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
