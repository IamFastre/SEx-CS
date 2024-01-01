using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticFunctionBodyStatement : SemanticStatement
{
    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.FunctionBodyStatement;

    public SemanticFunctionBodyStatement(Span span)
    {
        Span = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        throw new NotImplementedException();
    }
}