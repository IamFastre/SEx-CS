using SEx.Generic;
using SEx.Lex;
using SEx.AST;
using SEx.Diagnose;

namespace SEx.Parse;

public class Parser
{
    public List<Token> Tokens;
    public SyntaxTree Tree;
    public Diagnostics Diagnostics;
    private int Index;

    private Token Peek(int i = 1)
        => Index + i < Tokens.Count ? Tokens[Index + i] : Tokens[^1];

    private bool EOF => Current.Kind == TokenKind.EOF;
    private Token Current => Peek(0);

    public Parser(string source, Diagnostics? diagnostics = null)
    {
        // If no diagnostics object is passed, create a new one
        Diagnostics = diagnostics ?? new Diagnostics();
        var lexer = new Lexer(source, Diagnostics);

        Tokens = new();
        foreach (var tk in lexer.Tokens)
            if (!tk.Kind.IsParserIgnorable())
                Tokens.Add(tk);

        Tree = Parse();
    }

    public Parser(Lexer lexer)
    {
        Diagnostics = lexer.Diagnostics;

        Tokens = new();
        foreach (var tk in lexer.Tokens)
            if (!tk.Kind.IsParserIgnorable())
                Tokens.Add(tk);

        Tree = Parse();
    }

    /// <summary>
    /// Return the current token and increments the index by one
    /// </summary>
    private Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    private void Except(string message, ExceptionType type = ExceptionType.SyntaxError, Span? span = null)
    {
        span ??= Current.Span;

        Diagnostics.Add(
            type,
            message,
            span
        );
    }

    /// <summary>
    /// The Expect function checks if the current token matches the expected token type and returns the
    /// current token if it does, otherwise it fabricates a new token with the expected type copying the
    /// current token span
    /// </summary>
    private Token Expect(TokenKind kind, string? message = null)
    {
        if (Current.Kind == kind)
            return Eat();

        message ??= EOF ? "Expression not expected to end yet" : $"Unexpected \"{Current.Kind}\"";
        Except(message);

        return new Token(null, kind, Current.Span);
    }

    private Expression? Primary()
    {
        if (EOF)
            return null;

        switch (Current.Kind)
        {
            case TokenKind.Boolean:
                return new BooleanLiteral(Eat());

            case TokenKind.Integer:
                return new IntLiteral(Eat());

            case TokenKind.Float:
                return new FloatLiteral(Eat());

            case TokenKind.Char:
                return new CharLiteral(Eat());

            case TokenKind.String:
                return new StringLiteral(Eat());

            case TokenKind.Identifier:
                return new IdentifierLiteral(Eat());


            case TokenKind.OpenParenthesis:
                return Parenthesized();

            default:
                Except($"Invalid syntax: {Current.Value}");
                Eat();
                return Primary();
        }
    }

    private ParenExpression? Parenthesized()
    {
        var openParen = Eat();
        var expression = Current.Kind != TokenKind.CloseParenthesis ? Expression() : null;
        var closeParen = Expect(TokenKind.CloseParenthesis, $"'(' was never closed");

        if (expression is null)
            Except($"Expression expected before close parenthesis");

        return new ParenExpression(openParen, expression, closeParen);
    }

    private Expression? Expression(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = Primary();
        else
        {
            var uOp = Eat();
            left = Expression(unaryPrecedence);
            if (left is null)
            {
                Except($"Expected expression after operator {uOp.Value}", ExceptionType.SyntaxError, uOp.Span);
                return null;
            }
            left = new UnaryExpression(uOp, left);
        }

        while (!!!false)
        {
            var binaryPrecedence = Current.Kind.BinaryPrecedence();
            if (binaryPrecedence == 0 || binaryPrecedence <= parentPrecedence)
                break;

            var binOp = Eat();
            var right = Expression(binaryPrecedence);

            if (right is null)
            {
                Except($"Expected expression after operator {binOp.Value}", ExceptionType.SyntaxError, binOp.Span);
                return null;
            }

            left = new BinaryExpression(left!, binOp, right!);
        }

        return left;
    }

    public SyntaxTree Parse()
    {
        var program = Expression();
        var eof = Expect(TokenKind.EOF);

        return new SyntaxTree(program, Diagnostics, eof);
    }
}