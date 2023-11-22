using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class BlockStatement : Statement
{
    public Token OpenBrace   { get; }
    public Statement[] Body { get; }
    public Token CloseBrace  { get; }

    public BlockStatement(Token openBrace, Statement[] statements, Token closeBrace)
    {
        OpenBrace  = openBrace;
        Body       = statements;
        CloseBrace = closeBrace;

        Span = statements.Length > 0
             ? new(Body.First().Span, Body.Last().Span)
             : new();
        Kind = NodeKind.BlockStatement;
    }

    public override string ToString() => $"<{C.BLUE2}BlockStatement{C.GREEN2}[{Body.Length}]{C.END}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenBrace;

        foreach (var expr in Body)
            yield return expr;

        yield return CloseBrace;
    }
}
