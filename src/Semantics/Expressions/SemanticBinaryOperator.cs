using SEx.Evaluate.Values;
using SEx.Lex;

namespace SEx.Semantics;

internal enum BinaryOperationKind
{
    NullishCoalescence,

    Equality,
    Inequality,

    LAND,
    LOR,

    AND,
    OR,
    XOR,

    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    Modulo,

    RangeInclusion,

    CharAddition,
    CharSubtraction,

    StringConcatenation,
    StringMultiplication,
    StringInclusion,
    Greater,
    Less,
    GreaterEqual,
    LessEqual,
}

internal class SemanticBinaryOperator
{
    public ValType LeftType             { get; }
    public ValType RightType            { get; }
    public ValType ResultType           { get; }

    public BinaryOperationKind Kind { get; }
    public TokenKind Token          { get; }

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, ValType type)
        : this(token, kind, type, type, type) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, ValType operands, ValType result)
        : this(token, kind, operands, operands, result) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, ValType left, ValType right, ValType result)
    {
        LeftType   = left;
        RightType  = right;
        ResultType = result;

        Kind  = kind;
        Token = token;
    }

    public static SemanticBinaryOperator? GetSemanticOperator(ValType left, BinaryOperationKind opKind, ValType right)
    {
        foreach (var op in operators)
            if (op.LeftType.HasFlag(left)
             && op.Kind.HasFlag(opKind)
             && op.RightType.HasFlag(right))
                return op;

        return null;
    }

    private static readonly SemanticBinaryOperator[] operators =
    {
        new(TokenKind.NullishCoalescing, BinaryOperationKind.NullishCoalescence, ValType.Any),

        new(TokenKind.EqualEqual, BinaryOperationKind.Equality, ValType.Any, ValType.Boolean),
        new(TokenKind.NotEqual, BinaryOperationKind.Inequality, ValType.Any, ValType.Boolean),


        new(TokenKind.LogicalAND, BinaryOperationKind.LAND, ValType.Boolean),
        new(TokenKind.LogicalOR, BinaryOperationKind.LOR, ValType.Boolean),

        new(TokenKind.AND, BinaryOperationKind.AND, ValType.Boolean),
        new(TokenKind.OR, BinaryOperationKind.OR, ValType.Boolean),


        new(TokenKind.Greater, BinaryOperationKind.Greater, ValType.Number, ValType.Boolean),
        new(TokenKind.Less, BinaryOperationKind.Less, ValType.Number, ValType.Boolean),
        new(TokenKind.GreaterEqual, BinaryOperationKind.GreaterEqual, ValType.Number, ValType.Boolean),
        new(TokenKind.LessEqual, BinaryOperationKind.LessEqual, ValType.Number, ValType.Boolean),

        new(TokenKind.AND, BinaryOperationKind.AND, ValType.Integer),
        new(TokenKind.OR, BinaryOperationKind.OR, ValType.Integer),
        new(TokenKind.XOR, BinaryOperationKind.XOR, ValType.Integer),

        new(TokenKind.Plus, BinaryOperationKind.Addition, ValType.Integer),
        new(TokenKind.Plus, BinaryOperationKind.Addition, ValType.Float),
        new(TokenKind.Plus, BinaryOperationKind.Addition, ValType.Number, ValType.Float),

        new(TokenKind.Minus, BinaryOperationKind.Subtraction, ValType.Integer),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, ValType.Float),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, ValType.Number, ValType.Float),

        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, ValType.Integer),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, ValType.Float),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, ValType.Number, ValType.Float),

        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, ValType.Integer, ValType.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, ValType.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, ValType.Number, ValType.Float),

        new(TokenKind.Power, BinaryOperationKind.Power, ValType.Integer, ValType.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, ValType.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, ValType.Number, ValType.Float),

        new(TokenKind.Percent, BinaryOperationKind.Modulo, ValType.Integer),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, ValType.Float),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, ValType.Number, ValType.Float),


        new(TokenKind.InOperator, BinaryOperationKind.RangeInclusion, ValType.Number, ValType.Range, ValType.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.CharAddition, ValType.Char, ValType.Integer, ValType.Char),
        new(TokenKind.Plus, BinaryOperationKind.CharAddition, ValType.Integer, ValType.Char, ValType.Char),

        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, ValType.Char, ValType.Integer, ValType.Char),
        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, ValType.Integer, ValType.Char, ValType.Char),

        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, ValType.Char, ValType.String),


        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, ValType.String, ValType.Any, ValType.String),
        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, ValType.Any, ValType.String, ValType.String),

        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, ValType.Integer, ValType.String, ValType.String),
        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, ValType.String, ValType.Integer, ValType.String),


        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, ValType.String, ValType.String, ValType.Boolean),
        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, ValType.Char, ValType.String, ValType.Boolean),
    };
}