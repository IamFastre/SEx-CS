using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

internal class SeparatedClause : Clause
{
    public Expression[] Expressions { get; }
    public Token[]      Separators  { get; }

    public override Span     Span   { get; }
    public override NodeKind Kind => NodeKind.SeparatedClause;

    public SeparatedClause(Expression[] exprs, Token[] separators)
    {
        Expressions = exprs;
        Separators  = separators;

        Span        = exprs.Length > 0
                    ? new(exprs.First().Span, exprs.Last().Span)
                    : new();
    }

    public Expression this[int i] => Expressions[i];

    public static readonly SeparatedClause Empty = new(Array.Empty<Expression>(), Array.Empty<Token>());

    public override IEnumerable<Node> GetChildren()
    {
        int n = 0, s = 0;
        for (int i = 0; i < Expressions.Length + Separators.Length; i++)
        {
            if (int.IsEvenInteger(i))
                yield return Expressions[n++];
            else
                yield return Separators[s++].Node;
        }
    }
}