using SEx.Generic;
using SEx.Lex;
using SEx.AST;
using SEx.Analysis;

namespace SEx.Parse;

public class Parser
{
    public List<Token> Tokens;
    public SyntaxTree Tree;
    public Diagnostics Diagnostics;
    private int Index;

    private Token Peek(int i = 1)
        => Index + i < Tokens.Count ? Tokens[Index + i] : Tokens[^1];

    public readonly TokenType[] ParserIgnored =
    {
        TokenType.WhiteSpace,
        TokenType.Comment,
        TokenType.Bad
    };

    private bool EOF => Current.Type == TokenType.EOF;
    private Token Current => Peek(0);

    public Parser(string source, Diagnostics? diagnostics = null)
    {
        // If no diagnostics object is passed, create a new one
        Diagnostics = diagnostics ?? new Diagnostics();
        var lexer = new Lexer(source, Diagnostics).Initialize();

        Tokens = new();
        foreach (var tk in lexer.Tokens)
        {
            if (!ParserIgnored.Contains(tk.Type))
                Tokens.Add(tk);
        }

        Tree = Parse();
    }

    /// <summary>
    /// Return the current token and increments the index by one
    /// </summary>
    public Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    public void Except(string message, ExceptionType type = ExceptionType.SyntaxError, Span? span = null)
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
    public Token Expect(TokenType type, string? message = null)
    {
        if (Current.Type == type)
            return Eat();

        message ??= EOF ? "Expression not expected to end yet" : $"Unexpected \"{Current.Type}\"";
        Except(message);

        return new Token(null, type, Current.Span);
    }

    public Expression? Factor()
    {
        if (EOF)
            return null;

        switch (Current.Type)
        {
            case TokenType.Integer:
                return new IntLiteral(Eat());

            case TokenType.Float:
                return new FloatLiteral(Eat());

            case TokenType.String:
                return new StringLiteral(Eat());

            case TokenType.Char:
                return new CharLiteral(Eat());

            case TokenType.Identifier:
                return new IdentifierLiteral(Eat());

            case TokenType.OpenParenthesis:
                return Parenthesized();

            default:
                Except($"Invalid syntax: {Current.Value}");
                Eat();
                return Factor();
        }
    }

    private ParenExpression? Parenthesized()
    {
        var openParen = Eat();
        var expression = Current.Type != TokenType.CloseParenthesis ? Expression() : null;
        var closeParen = Expect(TokenType.CloseParenthesis, $"'(' was never closed");

        if (expression is null)
            Except($"Expression expected before close parenthesis");

        return new ParenExpression(openParen, expression, closeParen);
    }

    public Expression? Multiplicative()
    {
        var left = Factor();

        while (!EOF && Checker.Multiplicative.Contains(Current.Value))
        {
            var op = Eat();
            var right = Factor();

            left = new BinaryExpression(left!, op, right!);
        }

        return left;
    }

    public Expression? Additive()
    {
        var left = Multiplicative();

        while (!EOF && Checker.Additive.Contains(Current.Value))
        {
            var op = Eat();
            var right = Multiplicative();

            left = new BinaryExpression(left!, op, right!);
        }

        return left;
    }

    public Expression? Expression()
    {
        return Additive();
    }

    public SyntaxTree Parse()
    {
        var program = Expression();
        var eof = Expect(TokenType.EOF);

        return new SyntaxTree(program, Diagnostics, eof);
    }
}

static class Checker
{
    public static string[] Multiplicative = { "*", "/", "%", "^", "**" };
    public static string[] Additive       = { "+", "-" };

    public static char[] OpnBrackets = { '(', '[', '{', '<' };
    public static char[] ClsBrackets = { ')', ']', '}', '>' };

    public static char GetOtherPair(char C)
    {
        if (OpnBrackets.Contains(C))
            return ClsBrackets[Array.IndexOf(OpnBrackets, C)];
        if (ClsBrackets.Contains(C))
            return OpnBrackets[Array.IndexOf(ClsBrackets, C)];

        throw new Exception($"Char \"{C}\" seems to not having a pair.");
    }
}
