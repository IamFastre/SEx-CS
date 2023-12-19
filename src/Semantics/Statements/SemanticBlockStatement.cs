using SEx.Generic.Text;

namespace SEx.Semantics;

public sealed class SemanticBlockStatement : SemanticStatement
{
    public SemanticStatement[]   Body { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.BlockStatement;

    public SemanticBlockStatement(SemanticStatement[] statements, Span span)
    {
        Body = statements;
        Span = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Body;
}
