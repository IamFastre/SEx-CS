using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public class SemanticReturnStatement : SemanticStatement
{
    public SemanticExpression? Expression { get; }
    public TypeSymbol          Type       => Expression?.Type ?? TypeSymbol.Void;

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.ReturnStatement;

    public SemanticReturnStatement(SemanticExpression? expr, Span span)
    {
        Expression = expr;
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}