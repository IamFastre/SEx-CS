using SEx.Lexing;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public enum BinaryOperationKind
{
    NullishCoalescence,

    Equality,
    Inequality,

    LogicalAND,
    LogicalOR,

    BitwiseAND,
    BitwiseOR,
    BitwiseXOR,

    Greater,
    Less,
    GreaterEqual,
    LessEqual,

    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    Modulo,

    CharAddition,
    CharSubtraction,

    RangeInclusion,

    StringConcatenation,
    StringMultiplication,
    StringInclusion,

    ListConcatenation,
    ListMultiplication,
    ListInclusion,
}

public class SemanticBinaryOperator
{
    public TypeSymbol LeftType             { get; }
    public TypeSymbol RightType            { get; }
    public TypeSymbol ResultType           { get; private set; }

    public BinaryOperationKind Kind { get; }

    public SemanticBinaryOperator(BinaryOperationKind kind, TypeSymbol type)
        : this(kind, type, type, type) {}

    public SemanticBinaryOperator(BinaryOperationKind kind, TypeSymbol operands, TypeSymbol result)
        : this(kind, operands, operands, result) {}

    public SemanticBinaryOperator(BinaryOperationKind kind, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        LeftType   = left;
        RightType  = right;
        ResultType = result;
        Kind       = kind;
    }

    public static SemanticBinaryOperator GetSemanticOperator(TypeSymbol left, BinaryOperationKind opKind, TypeSymbol right)
    {
        foreach (var op in operators)
            if (op.LeftType.Matches(left) && op.Kind.HasFlag(opKind) && op.RightType.Matches(right))
            {
                if (op.ResultType is GenericTypeSymbol)
                {
                    if (left is not GenericTypeSymbol)
                        op.ResultType = right;
                    else if (right is not GenericTypeSymbol)
                        op.ResultType = left;
                    else
                        op.ResultType = left.Matches(right)
                                      ? left
                                      : right;
                }

                if (op.Kind is BinaryOperationKind.NullishCoalescence)
                    op.ResultType = left.Matches(right)
                                  ? left
                                  : right;

                return op;
            }

        throw new Exception("Where is that binary operation dude?!");
    }

    private static readonly SemanticBinaryOperator[] operators =
    {
        new(BinaryOperationKind.NullishCoalescence, TypeSymbol.Any),

        new(BinaryOperationKind.Equality, TypeSymbol.Any, TypeSymbol.Boolean),
        new(BinaryOperationKind.Inequality, TypeSymbol.Any, TypeSymbol.Boolean),


        new(BinaryOperationKind.LogicalAND, TypeSymbol.Boolean),
        new(BinaryOperationKind.LogicalOR, TypeSymbol.Boolean),

        new(BinaryOperationKind.BitwiseAND, TypeSymbol.Boolean),
        new(BinaryOperationKind.BitwiseOR, TypeSymbol.Boolean),
        new(BinaryOperationKind.BitwiseXOR, TypeSymbol.Boolean),

        new(BinaryOperationKind.BitwiseXOR, TypeSymbol.Boolean, TypeSymbol.Integer, TypeSymbol.Integer),
        new(BinaryOperationKind.BitwiseXOR, TypeSymbol.Integer, TypeSymbol.Boolean, TypeSymbol.Integer),

        new(BinaryOperationKind.BitwiseAND, TypeSymbol.Integer),
        new(BinaryOperationKind.BitwiseOR, TypeSymbol.Integer),
        new(BinaryOperationKind.BitwiseXOR, TypeSymbol.Integer),

        new(BinaryOperationKind.Greater, TypeSymbol.Number, TypeSymbol.Boolean),
        new(BinaryOperationKind.Less, TypeSymbol.Number, TypeSymbol.Boolean),
        new(BinaryOperationKind.GreaterEqual, TypeSymbol.Number, TypeSymbol.Boolean),
        new(BinaryOperationKind.LessEqual, TypeSymbol.Number, TypeSymbol.Boolean),

        new(BinaryOperationKind.Greater, TypeSymbol.Char, TypeSymbol.Boolean),
        new(BinaryOperationKind.Less, TypeSymbol.Char, TypeSymbol.Boolean),
        new(BinaryOperationKind.GreaterEqual, TypeSymbol.Char, TypeSymbol.Boolean),
        new(BinaryOperationKind.LessEqual, TypeSymbol.Char, TypeSymbol.Boolean),

        new(BinaryOperationKind.Addition, TypeSymbol.Integer),
        new(BinaryOperationKind.Addition, TypeSymbol.Float),
        new(BinaryOperationKind.Addition, TypeSymbol.Number, TypeSymbol.Float),

        new(BinaryOperationKind.Subtraction, TypeSymbol.Integer),
        new(BinaryOperationKind.Subtraction, TypeSymbol.Float),
        new(BinaryOperationKind.Subtraction, TypeSymbol.Number, TypeSymbol.Float),

        new(BinaryOperationKind.Multiplication, TypeSymbol.Integer),
        new(BinaryOperationKind.Multiplication, TypeSymbol.Float),
        new(BinaryOperationKind.Multiplication, TypeSymbol.Number, TypeSymbol.Float),

        new(BinaryOperationKind.Division, TypeSymbol.Integer, TypeSymbol.Float),
        new(BinaryOperationKind.Division, TypeSymbol.Float),
        new(BinaryOperationKind.Division, TypeSymbol.Number, TypeSymbol.Float),

        new(BinaryOperationKind.Power, TypeSymbol.Integer, TypeSymbol.Float),
        new(BinaryOperationKind.Power, TypeSymbol.Float),
        new(BinaryOperationKind.Power, TypeSymbol.Number, TypeSymbol.Float),

        new(BinaryOperationKind.Modulo, TypeSymbol.Integer),
        new(BinaryOperationKind.Modulo, TypeSymbol.Float),
        new(BinaryOperationKind.Modulo, TypeSymbol.Number, TypeSymbol.Float),


        new(BinaryOperationKind.RangeInclusion, TypeSymbol.Number, TypeSymbol.Range, TypeSymbol.Boolean),


        new(BinaryOperationKind.CharAddition, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),
        new(BinaryOperationKind.CharAddition, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),

        new(BinaryOperationKind.CharSubtraction, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),

        new(BinaryOperationKind.StringConcatenation, TypeSymbol.Char, TypeSymbol.String),


        new(BinaryOperationKind.StringConcatenation, TypeSymbol.String, TypeSymbol.Any, TypeSymbol.String),
        new(BinaryOperationKind.StringConcatenation, TypeSymbol.Any, TypeSymbol.String, TypeSymbol.String),

        new(BinaryOperationKind.StringMultiplication, TypeSymbol.Integer, TypeSymbol.String, TypeSymbol.String),
        new(BinaryOperationKind.StringMultiplication, TypeSymbol.String, TypeSymbol.Integer, TypeSymbol.String),


        new(BinaryOperationKind.StringInclusion, TypeSymbol.String, TypeSymbol.String, TypeSymbol.Boolean),
        new(BinaryOperationKind.StringInclusion, TypeSymbol.Char, TypeSymbol.String, TypeSymbol.Boolean),


        new(BinaryOperationKind.ListConcatenation, TypeSymbol.List),
        new(BinaryOperationKind.ListMultiplication, TypeSymbol.Integer, TypeSymbol.List, TypeSymbol.List),
        new(BinaryOperationKind.ListMultiplication, TypeSymbol.List, TypeSymbol.Integer, TypeSymbol.List),
        new(BinaryOperationKind.ListInclusion, TypeSymbol.Any, TypeSymbol.List, TypeSymbol.Boolean),
    };
}