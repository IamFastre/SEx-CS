using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

public sealed class ElseClause : Clause
{
    public Token     Else  { get; }
    public Statement Body  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ElseClause;

    public ElseClause(Token @else, Statement body)
    {
        Else = @else;
        Body = body;
        Span = new(@else.Span, body.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Else.Node;
        yield return Body;
    }
}
