using SEx.Lexing;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parsing;

public class SeparatedClause<T> : Clause where T : Node
{
    public T[]  Nodes      { get; }
    public Token[] Separators { get; }

    public override Span     Span   { get; }
    public override NodeKind Kind => NodeKind.SeparatedClause;

    public SeparatedClause(T[] exprs, Token[] separators)
    {
        Nodes = exprs;
        Separators  = separators;

        Span        = exprs.Length > 0
                    ? new(exprs.First().Span, exprs.Last().Span)
                    : new();
    }

    public Node this[int i] => Nodes[i];

    public static SeparatedClause<T> Empty => new(Array.Empty<T>(), Array.Empty<Token>());

    public override IEnumerable<Node> GetChildren()
    {
        int n = 0, s = 0;
        for (int i = 0; i < Nodes.Length + Separators.Length; i++)
        {
            if (int.IsEvenInteger(i))
                yield return Nodes[n++];
            else
                yield return Separators[s++].Node;
        }
    }
}