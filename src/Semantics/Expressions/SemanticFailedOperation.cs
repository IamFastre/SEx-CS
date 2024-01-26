using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticFailedOperation : SemanticExpression
{
    public SemanticExpression[] Expressions { get; }

    public override Span         Span       { get; }
    public override SemanticKind Kind => SemanticKind.FailedOperation;

    public SemanticFailedOperation(IEnumerable<SemanticExpression> expressions, Span? span = null)
        : base(TypeSymbol.Unknown)
    {
        Expressions = expressions.ToArray();
        Span        = span ?? new(Expressions[0].Span.Start, Expressions[^1].Span.End);
    }

    public override IEnumerable<SemanticNode> GetChildren() => Expressions;
}
