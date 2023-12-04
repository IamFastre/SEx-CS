using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticFailedExpressions : SemanticExpression
{

    public SemanticExpression[] Expressions { get; }
    public override Span Span               { get; }
    public override ValType Type      => ValType.Unknown;
    public override SemanticKind Kind => SemanticKind.FailedExpressions;

    public SemanticFailedExpressions(SemanticExpression[] expressions)
    {
        Expressions = expressions;
        Span = new Span(expressions[0].Span.Start, expressions[^1].Span.End);
    }
}

internal sealed class SemanticFailedExpression : SemanticExpression
{
    public override Span Span { get; }

    public override ValType Type      => ValType.Unknown;
    public override SemanticKind Kind => SemanticKind.FailedExpression;

    public SemanticFailedExpression(Span span) => Span = span;

}