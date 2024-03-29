using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public class TypeClause : Clause
{
    public Token Type          { get; }
    public int   ListDimension { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.TypeClause;

    public TypeClause(Token type, Span endSpan, int dimension = 0)
    {
        Type          = type;
        ListDimension = dimension;

        Span          = new(type.Span, endSpan);
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
