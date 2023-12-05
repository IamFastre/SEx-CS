using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping;

namespace SEx.Semantics;

internal enum UnaryOperationKind
{
    Identity,
    Negation,
    Complement,
}

internal sealed class SemanticUnaryOperation : SemanticExpression
{
    public SemanticExpression Operand       { get; }
    public UnaryOperationKind OperationKind { get; }

    public override Span         Span       { get; }
    public override TypeSymbol   Type => Operand.Type;
    public override SemanticKind Kind => SemanticKind.UnaryOperation;

    public SemanticUnaryOperation(SemanticExpression operand, UnaryOperationKind kind, Span span)
    {
        Operand       = operand;
        OperationKind = kind;

        Span          = span;
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
