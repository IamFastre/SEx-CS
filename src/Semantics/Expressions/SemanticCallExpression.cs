using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public class SemanticCallExpression : SemanticExpression
{
    public SemanticExpression   Function  { get; }
    public SemanticExpression[] Arguments { get; }

    public override Span         Span     { get; }
    public override SemanticKind Kind => SemanticKind.CallExpression;

    public SemanticCallExpression(SemanticExpression fs, TypeSymbol returnType, SemanticExpression[] arguments, Span span)
        : base(returnType)
    {
        Function  = fs;
        Arguments = arguments;

        Span      = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Function;
        foreach (var arg in Arguments)
            yield return arg;
    }
}
