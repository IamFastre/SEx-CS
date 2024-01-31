using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public class SemanticCallExpression : SemanticExpression
{
    public SemanticExpression   Function  { get; }
    public SemanticExpression[] Arguments { get; }

    public override Span         Span     { get; }
    public override SemanticKind Kind => SemanticKind.CallExpression;

    public SemanticCallExpression(SemanticExpression function, TypeSymbol returnType, IEnumerable<SemanticExpression> arguments, Span span)
        : base(returnType)
    {
        Function  = function;
        Arguments = arguments.ToArray();

        Span      = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Function;
        foreach (var arg in Arguments)
            yield return arg;
    }
}
