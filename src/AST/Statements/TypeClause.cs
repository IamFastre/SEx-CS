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

    public override IEnumerable<Node> GetChildren()
    {
        yield return Type.Node;
    }
}

internal class GenericTypeClause : TypeClause
{
    public TypeClause[] Parameters { get; }
    public override NodeKind Kind => NodeKind.GenericTypeClause;

    public GenericTypeClause(Token type, TypeClause[] parameters, Span end, int dimension = 0)
        : base(type, end, dimension)
        => Parameters = parameters;
}
