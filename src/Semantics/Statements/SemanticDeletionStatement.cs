using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public sealed class SemanticDeletionStatement : SemanticStatement
{
    public SemanticName          Name { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.DeletionStatement;

    public SemanticDeletionStatement(SemanticName name, Span span)
    {
        Name   = name;
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Name;
    }
}
