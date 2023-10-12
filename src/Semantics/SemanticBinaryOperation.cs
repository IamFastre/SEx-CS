using SEx.Evaluator.Values;
using SEx.Lex;

namespace SEx.Semantics;

internal enum BinaryOperationKind
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    Modulo,
    Concatenation,
    StringMultiplication,
    Inclusion
}

internal sealed class SemanticBinaryOperation : SemanticExpression
{
    public override SemanticKind Kind => SemanticKind.BinaryOperation;

    public SemanticExpression  Left          { get; }
    public BinaryOperationKind OperationKind { get; }
    public SemanticExpression  Right         { get; }

    // not final
    public override ValType Type => Left.Type;

    public SemanticBinaryOperation(SemanticExpression left, BinaryOperationKind operationKind, SemanticExpression right)
    {
        Left          = left;
        OperationKind = operationKind;
        Right         = right;
    }

    public static BinaryOperationKind? GetOperationKind(TokenKind op, ValType left, ValType right)
    {
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
        };

        if ((left, right).Match(ValType.String, ValType.Any, true))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.Concatenation;

        if ((left, right).Match(ValType.String, ValType.Whole, true))
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.StringMultiplication;

        if ((left, right).Match(ValType.String))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.Inclusion;

        return null;
    }
}