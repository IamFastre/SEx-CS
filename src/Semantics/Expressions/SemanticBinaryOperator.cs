using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal enum BinaryOperationKind
{
    NullishCoalescence,

    Equality,
    Inequality,

    LogicalAND,
    LogicalOR,

    BitwiseAND,
    BitwiseOR,
    BitwiseXOR,

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
    public TypeSymbol LeftType             { get; }
    public TypeSymbol RightType            { get; }
    public TypeSymbol ResultType           { get; private set; }

    public BinaryOperationKind Kind { get; }
    public TokenKind Token          { get; }

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeSymbol type)
        : this(token, kind, type, type, type) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeSymbol operands, TypeSymbol result)
        : this(token, kind, operands, operands, result) {}

    public SemanticBinaryOperator(TokenKind token, BinaryOperationKind kind, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        LeftType   = left;
        RightType  = right;
        ResultType = result;

        Kind  = kind;
        Token = token;
    }

    public static SemanticBinaryOperator GetSemanticOperator(TypeSymbol left, BinaryOperationKind opKind, TypeSymbol right)
    {
        foreach (var op in operators)
            if (op.LeftType.Matches(left) && op.Kind.HasFlag(opKind) && op.RightType.Matches(right))
            {
                if (op!.ResultType is GenericTypeSymbol)
                    op.ResultType = left;

                return op;
            }

        throw new Exception("Where is that binary operation dude?!");
    }

    private static readonly SemanticBinaryOperator[] operators =
    {
        new(TokenKind.NullishCoalescing, BinaryOperationKind.NullishCoalescence, TypeSymbol.Any),

        new(TokenKind.EqualEqual, BinaryOperationKind.Equality, TypeSymbol.Any, TypeSymbol.Boolean),
        new(TokenKind.NotEqual, BinaryOperationKind.Inequality, TypeSymbol.Any, TypeSymbol.Boolean),


        new(TokenKind.LogicalAND, BinaryOperationKind.LogicalAND, TypeSymbol.Boolean),
        new(TokenKind.LogicalOR, BinaryOperationKind.LogicalOR, TypeSymbol.Boolean),

        new(TokenKind.Ampersand, BinaryOperationKind.BitwiseAND, TypeSymbol.Boolean),
        new(TokenKind.Pipe, BinaryOperationKind.BitwiseOR, TypeSymbol.Boolean),
        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Boolean),

        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Boolean, TypeSymbol.Integer, TypeSymbol.Integer),
        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Integer, TypeSymbol.Boolean, TypeSymbol.Integer),

        new(TokenKind.Ampersand, BinaryOperationKind.BitwiseAND, TypeSymbol.Integer),
        new(TokenKind.Pipe, BinaryOperationKind.BitwiseOR, TypeSymbol.Integer),
        new(TokenKind.Caret, BinaryOperationKind.BitwiseXOR, TypeSymbol.Integer),

        new(TokenKind.Greater, BinaryOperationKind.Greater, TypeSymbol.Number, TypeSymbol.Boolean),
        new(TokenKind.Less, BinaryOperationKind.Less, TypeSymbol.Number, TypeSymbol.Boolean),
        new(TokenKind.GreaterEqual, BinaryOperationKind.GreaterEqual, TypeSymbol.Number, TypeSymbol.Boolean),
        new(TokenKind.LessEqual, BinaryOperationKind.LessEqual, TypeSymbol.Number, TypeSymbol.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Integer),
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Float),
        new(TokenKind.Plus, BinaryOperationKind.Addition, TypeSymbol.Number, TypeSymbol.Float),

        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Integer),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Float),
        new(TokenKind.Minus, BinaryOperationKind.Subtraction, TypeSymbol.Number, TypeSymbol.Float),

        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Integer),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Float),
        new(TokenKind.Asterisk, BinaryOperationKind.Multiplication, TypeSymbol.Number, TypeSymbol.Float),

        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Integer, TypeSymbol.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Float),
        new(TokenKind.ForwardSlash, BinaryOperationKind.Division, TypeSymbol.Number, TypeSymbol.Float),

        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Integer, TypeSymbol.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Float),
        new(TokenKind.Power, BinaryOperationKind.Power, TypeSymbol.Number, TypeSymbol.Float),

        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Integer),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Float),
        new(TokenKind.Percent, BinaryOperationKind.Modulo, TypeSymbol.Number, TypeSymbol.Float),


        new(TokenKind.InOperator, BinaryOperationKind.RangeInclusion, TypeSymbol.Number, TypeSymbol.Range, TypeSymbol.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.CharAddition, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),
        new(TokenKind.Plus, BinaryOperationKind.CharAddition, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),

        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, TypeSymbol.Char, TypeSymbol.Integer, TypeSymbol.Char),
        new(TokenKind.Minus, BinaryOperationKind.CharSubtraction, TypeSymbol.Integer, TypeSymbol.Char, TypeSymbol.Char),

        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeSymbol.Char, TypeSymbol.String),


        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeSymbol.String, TypeSymbol.Any, TypeSymbol.String),
        new(TokenKind.Plus, BinaryOperationKind.StringConcatenation, TypeSymbol.Any, TypeSymbol.String, TypeSymbol.String),

        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, TypeSymbol.Integer, TypeSymbol.String, TypeSymbol.String),
        new(TokenKind.Plus, BinaryOperationKind.StringMultiplication, TypeSymbol.String, TypeSymbol.Integer, TypeSymbol.String),


        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, TypeSymbol.String, TypeSymbol.String, TypeSymbol.Boolean),
        new(TokenKind.InOperator, BinaryOperationKind.StringInclusion, TypeSymbol.Char, TypeSymbol.String, TypeSymbol.Boolean),


        new(TokenKind.Plus, BinaryOperationKind.ListConcatenation, TypeSymbol.List),
        new(TokenKind.InOperator, BinaryOperationKind.ListInclusion, TypeSymbol.Any, TypeSymbol.List, TypeSymbol.Boolean),
    };
}