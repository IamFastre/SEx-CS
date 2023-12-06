using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

internal class TypeClause : Clause
{
    public Token Type          { get; }
    public int   ListDimension { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.TypeClause;

    public TypeClause(Token type, Span end, int dimension = 0)
    {
        Type          = type;
        ListDimension = dimension;

        Span          = new(type.Span, end);
    }

    public override string ToString() => $"<{Type.Value}>";

    public override IEnumerable<Node> GetChildren()
    {
        yield return Type.Node;
    }
}
