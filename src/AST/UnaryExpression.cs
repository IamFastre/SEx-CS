using SEx.Generic;
using SEx.Lex;

namespace SEx.AST;

public sealed class UnaryExpression : Expression
{
    public Token Operator;
    public Expression Operand;

    public UnaryExpression(Token @operator, Expression operand)
    {
        Operator = @operator;
        Operand = operand;
        Span = new Span(Operator.Span!.Start, Operand.Span!.End);
        Kind = NodeKind.UnaryOperation;
    }


    public override string ToString() => $"<Unary Operation: {Operator.Value} {Operand}>";
    public override IEnumerable<Node> GetChildren()
    {
        yield return Operator;
        yield return Operand;
    }
}