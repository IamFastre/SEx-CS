using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class IndexingExpression : Expression
{
    public Expression Iterable     { get; }
    public Token      OpenBracket  { get; }
    public Expression Index        { get; }
    public Token      CloseBracket { get; }

    public override Span     Span  { get; }
    public override NodeKind Kind => NodeKind.IndexingExpression;

    public IndexingExpression(Expression iterable, Token open, Expression expression, Token close)
    {
        Iterable     = iterable;
        OpenBracket  = open;
        Index        = expression;
        CloseBracket = close;

        Span         = new(Iterable.Span.Start, CloseBracket.Span.End);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Iterable;
        yield return OpenBracket.Node;
        yield return Index;
        yield return CloseBracket.Node;
    }
}
