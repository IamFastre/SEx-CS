using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal enum UnaryOperationKind
{
    Identity,
    Negation,
    Complement,
    IntComplement,
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

    public static UnaryOperationKind? GetOperationKind(TokenKind kind, TypeID operand)
    {

        if (operand is TypeID.Integer || operand is TypeID.Float)
            return kind switch
            {
                TokenKind.Plus  => UnaryOperationKind.Identity,
                TokenKind.Minus => UnaryOperationKind.Negation,
                _ => null,
            };

        if (operand is TypeID.Integer)
            return kind switch
            {
                TokenKind.Tilde => UnaryOperationKind.IntComplement,
                _ => null,
            };

        if (operand is TypeID.Boolean)
            return kind switch
            {
                TokenKind.ExclamationMark  => UnaryOperationKind.Complement,
                TokenKind.Tilde            => UnaryOperationKind.Complement,
                _ => null,
            };

        return null;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Operand;
    }
}
