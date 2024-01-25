using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class BlockStatement : Statement
{
    public Token OpenBrace        { get; }
    public Statement[] Body       { get; }
    public Token CloseBrace       { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.BlockStatement;

    public BlockStatement(Token openBrace, Statement[] statements, Token closeBrace)
    {
        OpenBrace  = openBrace;
        Body       = statements;
        CloseBrace = closeBrace;

        Span       = new(openBrace.Span, closeBrace.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenBrace.Node;

        foreach (var expr in Body)
            yield return expr;

        yield return CloseBrace.Node;
    }
}
