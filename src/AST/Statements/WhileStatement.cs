using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

internal sealed class WhileStatement : Statement
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

    public override string ToString()
        => $"<{C.BLUE2}WhileStatement{C.GREEN2}[{Body}]{(ElseClause is not null ? $"[{ElseClause}]" : "")}{C.END}>";

    public override IEnumerable<Node> GetChildren()
    {
        yield return While.Node;

        if (Condition is not null)
            yield return Condition;

        yield return Body;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}
