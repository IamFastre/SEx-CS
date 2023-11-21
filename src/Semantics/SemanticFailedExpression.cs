using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticFailedExpression : SemanticExpression
{
    public override ValType Type      => ValType.Unknown;
    public override SemanticKind Kind => SemanticKind.FailedExpression;

    public override Span Span { get; }
    public SemanticExpression[] Expressions { get; }

    public SemanticFailedExpression(SemanticExpression[] expressions)
    {
        Expressions = expressions;
        Span = new Span(expressions[0].Span.Start, expressions[^1].Span.End);
    }
}
