using Util;
using Lexing;
using AST;
using Diagnosing;

namespace Parsing;


public class Parser
{
    public List<Token> Tokens;
    public List<Statement> Tree;
    public Diagnostics Diagnostics;
    private int Index;

    private Token Peek(int i = 1)
        => Index+i < Tokens.Count ? Tokens[Index+i] : Tokens[^1];


    public readonly TokenType[] ParserIgnored
        = {TokenType.WhiteSpace, TokenType.Comment, TokenType.Bad};


    private bool EOF
    {
        get
        {
            return Current.Type == TokenType.EOF;
        }
    }

    private Token Current
    {
        get
        {
            return Peek(0);
        }
    }

    public Parser(string source, Diagnostics? diagnostics = null)
    {
        // If no diagnostics object is passed, create a new one
        Diagnostics = diagnostics ?? new Diagnostics("<unknown>", source);
        Lexer lexer = new Lexer(source, Diagnostics).Initialize();

        Tokens = new();
        Tree   = new();


        foreach (var tk in lexer.Tokens)
        {
            if (!ParserIgnored.Contains(tk.Type))
                Tokens.Add(tk);
        }

        while (!EOF)
            Tree.Add(Expression()!);

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

    /// <summary>
    /// The Expect function checks if the current token matches the expected token type and returns the
    /// current token if it does, otherwise it fabricates a new token with the expected type copying the 
    /// current token span
    /// </summary>
    public Token Expect(TokenType type)
    {
        if (Current.Type == type)
            return Current;

        return new Token(null, type, Current.Span);
    } 


    public Expression? Factor()
    {

        if (EOF)
            return null;

        switch (Current.Type)
        {
            case TokenType.Integer:
                return new IntegerLiteral(Eat());

            case TokenType.Float:
                return new FloatLiteral(Eat());
        
            case TokenType.String:
                return new StringLiteral(Eat());
        
            case TokenType.Char:
                return new CharLiteral(Eat());

            case TokenType.Identifier:
                return new IdentifierLiteral(Eat());

            case TokenType.Bad:
                Diagnostics.NewError(ErrorType.SyntaxError, $"Unrecognized character: {Current.Value}", Current.Span);
                Eat();
                return Factor();

            default:
                Diagnostics.NewError(ErrorType.SyntaxError, $"Invalid character: {Current.Value}", Current.Span);
                Eat();
                return Factor();
        };
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

}

static class Checker
{
    public static string[] Multiplicative = {"*","/","%","^","**"};
    public static string[] Additive       = {"+","-"};
}