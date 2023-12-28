using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class BreakStatement : Statement
{
    public Token Break            { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.BreakStatement;

    public BreakStatement(Token @break)
    {
        Break = @break;
        Span  = @break.Span;
    }

    public override IEnumerable<Node> GetChildren() => new[] { Break.Node };
}
