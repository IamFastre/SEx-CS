using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

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
