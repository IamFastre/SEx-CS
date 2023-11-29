using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class IndexingExpression : Expression
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

        Span         = new Span(Iterable.Span.Start, CloseBracket.Span.End);
    }


    public override string ToString()
        => $"<{Iterable}{C.RED}[{C.END}{Index}{C.RED}]{C.END}>";

    public override IEnumerable<Node> GetChildren()
    {
        yield return Iterable;
        yield return OpenBracket.Node;
        yield return Index;
        yield return CloseBracket.Node;
    }
}
