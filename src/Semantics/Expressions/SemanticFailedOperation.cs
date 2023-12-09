using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticFailedOperation : SemanticExpression
{
    public SemanticExpression[] Expressions { get; }

    public override Span         Span       { get; }
    public override TypeSymbol   Type => TypeSymbol.Unknown;
    public override SemanticKind Kind => SemanticKind.FailedOperation;

    public SemanticFailedOperation(SemanticExpression[] expressions)
    {
        Expressions = expressions;
        Span = new Span(expressions[0].Span.Start, expressions[^1].Span.End);
    }

    public override IEnumerable<SemanticNode> GetChildren() => Expressions;
}
