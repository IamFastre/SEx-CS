using SEx.Generic.Text;
using SEx.Lexing;

namespace SEx.AST;

public sealed class UnaryOperation : Expression
{
    public Token      Operator { get; }
    public Expression Operand  { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.UnaryOperation;

    public UnaryOperation(Token @operator, Expression operand)
    {
        Operator = @operator;
        Operand  = operand;
        Span     = new Span(Operator.Span!.Start, Operand.Span.End);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Operator.Node;
        yield return Operand;
    }
}