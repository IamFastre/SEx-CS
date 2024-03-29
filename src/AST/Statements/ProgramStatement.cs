using SEx.Generic.Text;

namespace SEx.AST;

public sealed class ProgramStatement : Statement
{
    public Statement[] Body { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ProgramStatement;

    public ProgramStatement(IEnumerable<Statement> statements)
    {
        Body = statements.ToArray();

        Span = Body.Length > 0
             ? new(Body.First().Span, Body.Last().Span)
             : new();
    }

    public override IEnumerable<Node> GetChildren() => Body;
}
