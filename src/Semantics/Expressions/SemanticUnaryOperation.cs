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
    BitwiseComplement,
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
        {
            if (kind is TokenKind.Plus)
                return UnaryOperationKind.Identity;
            if (kind is TokenKind.Minus)
                return UnaryOperationKind.Negation;
        }

        if (operand is TypeID.Integer)
        {
            if (kind is TokenKind.Tilde)
                return UnaryOperationKind.BitwiseComplement;
        }

        if (operand is TypeID.Boolean)
        {
            if (kind is TokenKind.BangMark or TokenKind.Tilde)
                return UnaryOperationKind.Complement;
            if (kind is TokenKind.Minus)
                return UnaryOperationKind.Negation;
        }

        return null;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Operand;
    }
}
