using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticStringFragment : SemanticClause
{
    public string Value { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.StringFragment;

    public SemanticStringFragment(string value, Span span)
    {
        Value = value;
        Span  = span;
    }

    public override IEnumerable<SemanticNode> GetChildren() => Array.Empty<SemanticNode>();
}