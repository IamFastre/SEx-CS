using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class FormatStringLiteral : Expression
{
    public Token        Opener      { get; }
    public Expression[] Expressions { get; }
    public Token        Closer      { get; }

    public override Span     Span   { get; }
    public override NodeKind Kind => NodeKind.FormatString;

    public FormatStringLiteral(Token opener, Expression[] expressions, Token closer)
    {
        Opener      = opener;
        Expressions = expressions;
        Closer      = closer;
        Span        = opener.Span;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Opener.Node;
        foreach (var expr in Expressions)
            yield return expr;
        yield return Closer.Node;
    }
}