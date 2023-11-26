using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticBinaryOperation : SemanticExpression
{
    public SemanticExpression     Left     { get; }
    public SemanticBinaryOperator Operator { get; }
    public SemanticExpression     Right    { get; }

    public override ValType Type { get; }
    public override Span    Span { get; }
    public override SemanticKind Kind => SemanticKind.BinaryOperation;

    public SemanticBinaryOperation(SemanticExpression left, BinaryOperationKind kind, SemanticExpression right)
    {
        Left     = left;
        Operator = SemanticBinaryOperator.GetSemanticOperator(left.Type, kind, right.Type)!;
        Right    = right;

        Type = Operator.ResultType;
        Span = new Span(left.Span.Start, right.Span.End);
    }

    public static BinaryOperationKind? GetOperationKind(TokenKind op, ValType left, ValType right)
    {
        if (op is TokenKind.NullishCoalescing)
            return BinaryOperationKind.NullishCoalescence;

        if ((left, right).Known())
        {
            if (op is TokenKind.EqualEqual)
                return BinaryOperationKind.Equality;
            if (op is TokenKind.NotEqual)
                return BinaryOperationKind.Inequality;
        }

        if ((left, right).Match(ValType.Integer))
        {
            if (op is TokenKind.AND)
                return BinaryOperationKind.AND;
            if (op is TokenKind.OR)
                return BinaryOperationKind.OR;
            if (op is TokenKind.XOR)
                return BinaryOperationKind.XOR;
        }

        if ((left, right).Match(ValType.Boolean))
        {
            if (op is TokenKind.LogicalAND or TokenKind.AND)
                return BinaryOperationKind.LAND;
            if (op is TokenKind.LogicalOR or TokenKind.OR)
                return BinaryOperationKind.LOR;
        }

        if ((left, right).Match(ValType.Number))
        {
            if (op is TokenKind.Plus)
                return BinaryOperationKind.Addition;
            if (op is TokenKind.Minus)
                return BinaryOperationKind.Subtraction;
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.Multiplication;
            if (op is TokenKind.ForwardSlash)
                return BinaryOperationKind.Division;
            if (op is TokenKind.Percent)
                return BinaryOperationKind.Modulo;
            if (op is TokenKind.Power)
                return BinaryOperationKind.Power;

            if (op is TokenKind.Greater)
                return BinaryOperationKind.Greater;
            if (op is TokenKind.Less)
                return BinaryOperationKind.Less;
            if (op is TokenKind.GreaterEqual)
                return BinaryOperationKind.GreaterEqual;
            if (op is TokenKind.LessEqual)
                return BinaryOperationKind.LessEqual;
        }


        if ((left, right).Match(ValType.Char, ValType.Integer, true))
        {
            if (op is TokenKind.Plus)
                return BinaryOperationKind.CharAddition;
            if (op is TokenKind.Minus)
                return BinaryOperationKind.CharSubtraction;
        }

        if ((left, right).Match(ValType.Char))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(ValType.String, ValType.Any, true))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(ValType.String, ValType.Whole, true))
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.StringMultiplication;

        if ((left, right).Match(ValType.String) || (left is ValType.Char && right is ValType.String))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.Inclusion;

        return null;
    }
}
