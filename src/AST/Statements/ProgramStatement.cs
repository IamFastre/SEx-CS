using SEx.Generic.Constants;
using SEx.Generic.Text;

namespace SEx.AST;

internal sealed class ProgramStatement : Statement
{
    public Statement[] Body { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ProgramStatement;

    public ProgramStatement(Statement[] statements)
    {
        Body = statements;

        Span = statements.Length > 0
             ? new(Body.First().Span, Body.Last().Span)
             : new();
    }

    public override string ToString() => $"<{C.BLUE2}ProgramStatement{C.GREEN2}[{Body.Length}]{C.END}>";
    public override IEnumerable<Node> GetChildren() => Body;
}
