using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticIndexAssignmentExpression : SemanticExpression
{
    public SemanticIndexingExpression Indexing   { get; }
    public SemanticExpression         Expression { get; }

    public override Span         Span            { get; }
    public override SemanticKind Kind => SemanticKind.IndexAssignment;

    public SemanticIndexAssignmentExpression(SemanticIndexingExpression indexExpr, SemanticExpression expr, Span span)
        : base(expr.Type)
    {
        Indexing   = indexExpr;
        Expression = expr;
        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        throw new NotImplementedException();
    }
}