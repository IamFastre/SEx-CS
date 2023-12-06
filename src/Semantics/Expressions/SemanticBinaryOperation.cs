using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal sealed class SemanticBinaryOperation : SemanticExpression
{
    public SemanticExpression     Left     { get; }
    public SemanticBinaryOperator Operator { get; }
    public SemanticExpression     Right    { get; }

    public override TypeSymbol Type { get; }
    public override Span       Span { get; }
    public override SemanticKind Kind => SemanticKind.BinaryOperation;

    public SemanticBinaryOperation(SemanticExpression left, BinaryOperationKind kind, SemanticExpression right)
    {
        Left     = left;
        Operator = SemanticBinaryOperator.GetSemanticOperator(left.Type.ID, kind, right.Type.ID)!;
        Right    = right;

        Type = TypeSymbol.GetTypeByID(Operator.ResultType)!;
        Span = new Span(left.Span.Start, right.Span.End);
    }

    public static BinaryOperationKind? GetOperationKind(TokenKind op, TypeSymbol left, TypeSymbol right)
    {
        if (op is TokenKind.NullishCoalescing)
            return BinaryOperationKind.NullishCoalescence;

        if (left.IsKnown && right.IsKnown)
        {
            if (op is TokenKind.EqualEqual)
                return BinaryOperationKind.Equality;
            if (op is TokenKind.NotEqual)
                return BinaryOperationKind.Inequality;
        }

        if ((left, right).Match(TypeSymbol.Integer))
        {
            if (op is TokenKind.AND)
                return BinaryOperationKind.AND;
            if (op is TokenKind.OR)
                return BinaryOperationKind.OR;
            if (op is TokenKind.XOR)
                return BinaryOperationKind.XOR;
        }

        if ((left, right).Match(TypeSymbol.Boolean))
        {
            if (op is TokenKind.LogicalAND or TokenKind.AND)
                return BinaryOperationKind.LAND;
            if (op is TokenKind.LogicalOR or TokenKind.OR)
                return BinaryOperationKind.LOR;
            if (op is TokenKind.XOR)
                return BinaryOperationKind.XOR;
        }

        if ((left, right).Match(TypeSymbol.Number))
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



        if ((left, right).Match(TypeSymbol.Number, TypeSymbol.Range))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.RangeInclusion;



        if ((left, right).Match(TypeSymbol.Char, TypeSymbol.Integer, true))
        {
            if (op is TokenKind.Plus)
                return BinaryOperationKind.CharAddition;
            if (op is TokenKind.Minus)
                return BinaryOperationKind.CharSubtraction;
        }

        if ((left, right).Match(TypeSymbol.Char))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(TypeSymbol.String, TypeSymbol.Any, true))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(TypeSymbol.String, TypeSymbol.Whole, true))
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.StringMultiplication;

        if ((left, right).Match(TypeSymbol.String) || (left == TypeSymbol.Char && right == TypeSymbol.String))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.StringInclusion;



        if ((left, right).Match(GenericTypeSymbol.List(left.ElementType!)))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.ListConcatenation;

        if ((left, right).Match(TypeSymbol.Any, GenericTypeSymbol.List(left.ElementType!)))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.ListInclusion;

        return null;
    }
}
