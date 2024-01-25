using SEx.Generic.Text;
using SEx.Lexing;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public sealed class SemanticBinaryOperation : SemanticExpression
{
    public SemanticExpression     Left     { get; }
    public SemanticBinaryOperator Operator { get; }
    public SemanticExpression     Right    { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.BinaryOperation;

    public SemanticBinaryOperation(SemanticExpression left, SemanticBinaryOperator @operator, SemanticExpression right)
        : base(@operator.ResultType)
    {
        Left     = left;
        Operator = @operator;
        Right    = right;

        Span = new Span(left.Span.Start, right.Span.End);
    }

    public static BinaryOperationKind? GetOperationKind(TokenKind op, TypeSymbol left, TypeSymbol right)
    {
        if (op is TokenKind.NullishCoalescing)
            if (left.Matches(right) || right.Matches(left))
                return BinaryOperationKind.NullishCoalescence;

        if (left.IsKnown && right.IsKnown)
        {
            if (op is TokenKind.EqualEqual)
                return BinaryOperationKind.Equality;
            if (op is TokenKind.NotEqual)
                return BinaryOperationKind.Inequality;
        }

        if ((left, right).Match(TypeSymbol.Integer)
        ||  (left, right).Match(TypeSymbol.Boolean)
        ||  (left, right).Match(TypeSymbol.Integer, TypeSymbol.Boolean, true))
        {
            if (op is TokenKind.Ampersand)
                return BinaryOperationKind.BitwiseAND;
            if (op is TokenKind.Pipe)
                return BinaryOperationKind.BitwiseOR;
            if (op is TokenKind.Caret)
                return BinaryOperationKind.BitwiseXOR;
        }

        if ((left, right).Match(TypeSymbol.Boolean))
        {
            if (op is TokenKind.LogicalAND or TokenKind.Ampersand)
                return BinaryOperationKind.LogicalAND;
            if (op is TokenKind.LogicalOR or TokenKind.Pipe)
                return BinaryOperationKind.LogicalOR;
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
        }

        if ((left, right).Match(TypeSymbol.Number) || (left, right).Match(TypeSymbol.Char))
        {
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
            if (op is TokenKind.Plus)
                return BinaryOperationKind.CharAddition;

        if ((left, right).Match(TypeSymbol.Char, TypeSymbol.Integer))
            if (op is TokenKind.Minus)
                return BinaryOperationKind.CharSubtraction;

        if ((left, right).Match(TypeSymbol.Char))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(TypeSymbol.String, TypeSymbol.Any, true))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.StringConcatenation;

        if ((left, right).Match(TypeSymbol.String, TypeSymbol.Integer, true))
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.StringMultiplication;

        if ((left, right).Match(TypeSymbol.String) || (left == TypeSymbol.Char && right == TypeSymbol.String))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.StringInclusion;



        if ((left, right).Match(TypeSymbol.List) && (left, right).Match(left.Matches(right) ? left : right))
            if (op is TokenKind.Plus)
                return BinaryOperationKind.ListConcatenation;

        if ((left, right).Match(TypeSymbol.List, TypeSymbol.Integer, true))
            if (op is TokenKind.Asterisk)
                return BinaryOperationKind.ListMultiplication;

        if ((left, right).Match(TypeSymbol.Any, TypeSymbol.TypedList(right.ElementType!)))
            if (op is TokenKind.InOperator)
                return BinaryOperationKind.ListInclusion;

        return null;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Left;
        yield return Right;
    }
}
