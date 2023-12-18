using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

public sealed class SemanticFailedOperation : SemanticExpression
{
    public SemanticExpression[] Expressions { get; }

    public override Span         Span       { get; }
    public override SemanticKind Kind => SemanticKind.FailedOperation;

    public SemanticFailedOperation(SemanticExpression[] expressions, Span? span = null)
        : base(TypeSymbol.Unknown)
    {
        Expressions = expressions;
        Span        = span ?? new(expressions[0].Span.Start, expressions[^1].Span.End);
    }

    public override IEnumerable<SemanticNode> GetChildren() => Expressions;
}
