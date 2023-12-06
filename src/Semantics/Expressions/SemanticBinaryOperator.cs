using SEx.Lex;
using SEx.Scoping.Symbols;

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

    ListConcatenation,
    ListInclusion,
}

internal class SemanticBinaryOperator
{
    public TypeID LeftType             { get; }
    public TypeID RightType            { get; }
    public TypeID ResultType           { get; }

    public BinaryOperationKind Kind { get; }
    public TokenKind Token          { get; }

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeID type)
        : this(token, kind, type, type, type) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeID operands, TypeID result)
        : this(token, kind, operands, operands, result) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeID left, TypeID right, TypeID result)
    {
        LeftType   = left;
        RightType  = right;
        ResultType = result;

        Kind  = kind;
        Token = token;
    }

    public static SemanticBinaryOperator? GetSemanticOperator(TypeID left, BinaryOperationKind opKind, TypeID right)
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
        new(TokenKind.NullishCoalescing, BinaryOperationKind.NullishCoalescence, TypeID.Any),

        new(TokenKind.EqualEqual, BinaryOperationKind.Equality, TypeID.Any, TypeID.Boolean),
        new(TokenKind.NotEqual, BinaryOperationKind.Inequality, TypeID.Any, TypeID.Boolean),


        new(TokenKind.LogicalAND, BinaryOperationKind.LAND, TypeID.Boolean),
        new(TokenKind.LogicalOR, BinaryOperationKind.LOR, TypeID.Boolean),

        new(TokenKind.AND, BinaryOperationKind.AND, TypeID.Boolean),
        new(TokenKind.OR, BinaryOperationKind.OR, TypeID.Boolean),
        new(TokenKind.XOR, BinaryOperationKind.XOR, TypeID.Boolean),


        new(TokenKind.Greater, BinaryOperationKind.Greater, TypeID.Number, TypeID.Boolean),
        new(TokenKind.Less, BinaryOperationKind.Less, TypeID.Number, TypeID.Boolean),
        new(TokenKind.GreaterEqual, BinaryOperationKind.GreaterEqual, TypeID.Number, TypeID.Boolean),
        new(TokenKind.LessEqual, BinaryOperationKind.LessEqual, TypeID.Number, TypeID.Boolean),

        new(TokenKind.AND, BinaryOperationKind.AND, TypeID.Integer),
        new(TokenKind.OR, BinaryOperationKind.OR, TypeID.Integer),
        new(TokenKind.XOR, BinaryOperationKind.XOR, TypeID.Integer),

        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeID.Integer),
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeID.Float),
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeID.Number, TypeID.Float),

        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeID.Integer),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeID.Float),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeID.Number, TypeID.Float),

        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeID.Integer),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeID.Float),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeID.Number, TypeID.Float),

        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeID.Integer, TypeID.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeID.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeID.Number, TypeID.Float),

        new(TokenKind.Power, BinaryOperationKind.Power, TypeID.Integer, TypeID.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, TypeID.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, TypeID.Number, TypeID.Float),

        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeID.Integer),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeID.Float),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeID.Number, TypeID.Float),


        new(TokenKind.InOperator, BinaryOperationKind.RangeInclusion, TypeID.Number, TypeID.Range, TypeID.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.CharAddition, TypeID.Char, TypeID.Integer, TypeID.Char),
        new(TokenKind.Plus, BinaryOperationKind.CharAddition, TypeID.Integer, TypeID.Char, TypeID.Char),

        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, TypeID.Char, TypeID.Integer, TypeID.Char),
        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, TypeID.Integer, TypeID.Char, TypeID.Char),

        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeID.Char, TypeID.String),


        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeID.String, TypeID.Any, TypeID.String),
        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeID.Any, TypeID.String, TypeID.String),

        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, TypeID.Integer, TypeID.String, TypeID.String),
        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, TypeID.String, TypeID.Integer, TypeID.String),


        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, TypeID.String, TypeID.String, TypeID.Boolean),
        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, TypeID.Char, TypeID.String, TypeID.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.ListConcatenation, TypeID.List),
        new(TokenKind.InOperator, BinaryOperationKind.ListInclusion, TypeID.Any, TypeID.List, TypeID.Boolean),
    };
}