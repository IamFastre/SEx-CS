using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class UnaryExpression : Expression
{
    public Token      Operator { get; }
    public Expression Operand  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.UnaryOperation;

    public UnaryExpression(Token @operator, Expression operand)
    {
        Operator = @operator;
        Operand  = operand;
        Span     = new Span(Operator.Span!.Start, Operand.Span.End);
    }


    public override string ToString() => $"<{C.BLUE2}UnaryOperation{C.END}: {C.GREEN2}{Operator.Value}{C.END} {Operand}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Operator.Node;
        yield return Operand;
    }
}