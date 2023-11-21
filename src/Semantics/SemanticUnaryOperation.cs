using SEx.Evaluate.Values;
using SEx.Generic.Text;
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

    public Token              Operator      { get; }
    public SemanticExpression Operand       { get; }

    public UnaryOperationKind OperationKind { get; }

    public override Span Span { get; }
    // not final
    public override ValType Type => Operand.Type;

    public SemanticUnaryOperation(Token @operator, SemanticExpression operand, UnaryOperationKind? kind = null)
    {
        Operator      = @operator;
        Operand       = operand;

        OperationKind = kind ?? GetOperationKind(@operator.Kind, operand.Type)!.Value;

        Span = new Span(@operator.Span.Start, Operand.Span.End);
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
