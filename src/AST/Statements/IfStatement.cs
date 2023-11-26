using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

internal sealed class IfStatement : Statement
{
    public Token       If         { get; }
    public Expression  Condition  { get; }
    public Statement   Then       { get; }
    public ElseClause? ElseClause { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.IfStatement;

    public IfStatement(Token @if, Expression condition, Statement body, ElseClause? elseClause = null)
    {
        If         = @if;
        Condition  = condition;
        Then       = body;
        ElseClause = elseClause;

        Span = new(@if.Span, elseClause is not null
                           ? elseClause.Span
                           : body.Span);
    }

    public override string ToString()
        => $"<{C.BLUE2}IfStatement{C.GREEN2}[{Then}]{(ElseClause is not null ? $"[{ElseClause}]" : "")}{C.END}>";

    public override IEnumerable<Node> GetChildren()
    {
        yield return If.Node;

        if (Condition is not null)
            yield return Condition;

        yield return Then;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}

internal sealed class ElseClause : Clause
{
    public override Span     Span { get; }
    public override NodeKind Kind { get; }

    public Token     Else  { get; }
    public Statement Body  { get; }

    public ElseClause(Token @else, Statement body)
    {
        Else = @else;
        Body = body;
        Span = new(@else.Span, body.Span);
        Kind = NodeKind.ElseClause;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Else.Node;
        yield return Body;
    }

    public override string ToString() => $"<{C.BLUE2}ElseClause{C.GREEN2}[{Body}]{C.END}>";
}
