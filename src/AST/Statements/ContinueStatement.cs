using SEx.Lexing;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parsing;

public class ContinueStatement : Statement
{
    public Token Continue         { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ContinueStatement;

    public ContinueStatement(Token @continue)
    {
        Continue = @continue;
        Span     = @continue.Span;
    }

    public override IEnumerable<Node> GetChildren() => new[] { Continue.Node };
}
