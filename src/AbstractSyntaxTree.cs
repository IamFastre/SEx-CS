using SEx.Generic;
using SEx.Lex;
using SEx.Analysis;

namespace SEx.AST;

public enum NodeType
{
    Bad,
    Unknown,
    Integer,
    Float,
    Char,
    String,
    Identifier,
    ParenExpression,
    BinaryOperation,
}

public class SyntaxTree
{
    public Statement? Root { get; }
    public Token EOF { get; }

    public SyntaxTree(Statement? root, Diagnostics diagnostics, Token eof)
    {
        Root = root;
        EOF = eof;
    }

    public override string ToString()
    {
        return $"{Root}";
    }
}

public abstract class Node
{
    public Span? Span;
    public abstract NodeType Type { get; }

    public abstract override string ToString();
}

public abstract class Statement : Node {}
public abstract class Expression : Statement {}

public abstract class Literal : Expression
{
    public Token? Token;
    public string? Value;

    public sealed override string ToString()
    {
        return $"{Type}: {Token!.Value}";
    }

    public string Full => $"{this} at {Span}";
    public static readonly UnknownLiteral Unknown = new(Token.Template);
}

public sealed class UnknownLiteral : Literal
{
    public new readonly byte? Value = null;
    public UnknownLiteral(Token token)
    {
        Token = token;
        Span = Token.Span;
    }

    public override NodeType Type => NodeType.Unknown;
}

public sealed class IntLiteral : Literal
{
    public IntLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span = token.Span;
    }

    public override NodeType Type => NodeType.Integer;
}

public sealed class FloatLiteral : Literal
{
    public FloatLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span = token.Span;
    }

    public override NodeType Type => NodeType.Float;
}

public sealed class CharLiteral : Literal
{
    public CharLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span = token.Span;
    }

    public override NodeType Type => NodeType.Char;
}

public sealed class StringLiteral : Literal
{
    public StringLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span = token.Span;
    }

    public override NodeType Type => NodeType.String;
}

public sealed class IdentifierLiteral : Literal
{
    public IdentifierLiteral(Token token)
    {
        Token = token;
        Value = token.Value!.ToString();
        Span = token.Span;
    }

    public override NodeType Type => NodeType.Identifier;
}

public sealed class ParenExpression : Expression
{
    public Token OpenParen;
    public Expression? Expression;
    public Token CloseParen;

    public ParenExpression(Token openParen, Expression? expression,Token closeParen)
    {
        OpenParen = openParen;
        Expression = expression;
        CloseParen = closeParen;

        Span = new Span(OpenParen.Span.Start, CloseParen.Span.End);
    }

    public override NodeType Type => NodeType.ParenExpression;

    public override string ToString()
    {
        return $"({Expression})";
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

        Span = new Span(LHS.Span!.Start, RHS.Span!.End);
    }

    public override NodeType Type => NodeType.BinaryOperation;

    public override string ToString()
    {
        return $"<BinOp: {LHS} {Operator.Value} {RHS}>";
    }
}