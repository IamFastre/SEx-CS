using SEx.AST;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Parse;

internal sealed class ListLiteral : Expression
{
    public Token        OpenBracket  { get; }
    public Expression[] Elements     { get; }
    public Token        CloseBracket { get; }

    public override Span     Span    { get; }
    public override NodeKind Kind  => NodeKind.List;

    public ListLiteral(Token open, Expression[] elements, Token close)
    {
        OpenBracket  = open;
        Elements     = elements;
        CloseBracket = close;

        Span         = new(open.Span, close.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenBracket.Node;
        foreach (var expr in Elements)
            yield return expr;
        yield return CloseBracket.Node;
    }
}