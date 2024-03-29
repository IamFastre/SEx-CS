using SEx.AST;
using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.Parsing;

public sealed class ListLiteral : Expression
{
    public Token                       OpenBracket  { get; }
    public SeparatedClause<Expression> Elements     { get; }
    public Token                       CloseBracket { get; }

    public override Span     Span                   { get; }
    public override NodeKind Kind  => NodeKind.List;

    public ListLiteral(Token open, SeparatedClause<Expression> elements, Token close)
    {
        OpenBracket  = open;
        Elements     = elements;
        CloseBracket = close;

        Span         = new(open.Span, close.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenBracket.Node;
        yield return Elements;
        yield return CloseBracket.Node;
    }
}