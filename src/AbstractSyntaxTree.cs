using Util;
using Lexing;

namespace AST;

public enum NodeType
{
    Bad,
    Number,
    Integer,
    Float,
    Char,
    String,
    Identifier,
    BinaryOperation,
}

public abstract class Node
{
    public Span? Span;
    public NodeType Type;

    public abstract override string ToString();
}

public abstract class Statement : Node {}
public abstract class Expression : Statement {}

public abstract class Literal : Expression
{
    public Token? Token;
    public dynamic? Value;

    public sealed override string ToString()
    {
        return $"({Type}: {Token!.Value})";
    }

    public string Full => $"{this} at {Span}";
}

public sealed class IntegerLiteral : Literal
{
    public IntegerLiteral(Token token)
    {
        Token = token;
        Value = int.Parse(token.Value!);
        Span = token.Span;
        Type = NodeType.Integer;
    }
}

public sealed class FloatLiteral : Literal
{
    public FloatLiteral(Token token)
    {
        Token = token;
        Value = float.Parse(token.Value!);
        Span = token.Span;
        Type = NodeType.Float;
    }
}

public sealed class CharLiteral : Literal
{
    public CharLiteral(Token token)
    {
        Token = token;
        Value = char.Parse(token.Value!);
        Span = token.Span;
        Type = NodeType.Char;
    }
}

public sealed class StringLiteral : Literal
{
    public StringLiteral(Token token)
    {
        Token = token;
        Value = token.Value![1..^1];
        Span = token.Span;
        Type = NodeType.String;
    }
}

public sealed class IdentifierLiteral : Literal
{
    public IdentifierLiteral(Token token)
    {
        Token = token;
        Value = token.Value!.ToString();
        Span = token.Span;
        Type = NodeType.Identifier;
    }
}

public sealed class BinaryExpression : Expression
{
    public Expression LHS;
    public Token Operator;
    public Expression RHS;

    public BinaryExpression(Expression leftHandExpr, Token binOperator, Expression rightHandExpr)
    {
        LHS = leftHandExpr;
        Operator = binOperator;
        RHS = rightHandExpr;

        Type = NodeType.BinaryOperation;
    }

    public override string ToString()
    {
        return $"(BinOp: {LHS} {Operator.Value} {RHS})";
    }
}