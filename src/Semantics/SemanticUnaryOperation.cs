using SEx.Evaluator.Values;
using SEx.Lex;

namespace SEx.Semantics;

internal enum UnaryOperationKind
{
    Identity,
    Negation,
    Complement,
    Incrementing,
    Decrementing
}

internal sealed class SemanticUnaryOperation : SemanticExpression
{
    public override SemanticKind Kind => SemanticKind.UnaryOperation;

    public UnaryOperationKind OperationKind { get; }
    public SemanticExpression Operand       { get; }

    // not final
    public override ValType Type => Operand.Type;

    public SemanticUnaryOperation(UnaryOperationKind operationKind, SemanticExpression operand)
    {
        OperationKind = operationKind;
        Operand       = operand;
    }

    public static UnaryOperationKind? GetOperationKind(TokenKind kind, ValType operand)
    {

        if (operand is ValType.Integer || operand is ValType.Float)
            return kind switch
            {
                TokenKind.Plus  => UnaryOperationKind.Identity,
                TokenKind.Minus => UnaryOperationKind.Negation,
                _ => null,
            };

        if (operand is ValType.Boolean)
            return kind switch
            {
                TokenKind.ExclamationMark  => UnaryOperationKind.Complement,
                _ => null,
            };

        return null;
    }
}
