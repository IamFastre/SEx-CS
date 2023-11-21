using SEx.Lex;
using SEx.AST;
using SEx.Diagnose;
using SEx.Generic.Constants;
using SEx.Generic.Text;

namespace SEx.Parse;

public class Parser
{
    public List<Token> Tokens      { get; }
    public Diagnostics Diagnostics { get; }
    public Statement?  Tree        { get; protected set; }

    private int Index;
    public readonly Source Source;

    private Token Peek(int i = 1) => Index + i < Tokens.Count ? Tokens[Index + i] : Tokens[^1];
    private Token Current         => Peek(0);
    private bool  EOF             => Current.Kind == TokenKind.EOF;

    public Parser(Lexer lexer)
    {
        Source      = lexer.Source;
        Diagnostics = lexer.Diagnostics;
        Tokens      = new();

        foreach (var tk in lexer.Tokens)
            if (!tk.Kind.IsParserIgnorable())
                Tokens.Add(tk);
    }

    public Statement Parse()
        => Tree = GetStatement() ?? ExpressionStatement.Empty(Tokens[^1].Span);

    private Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    private void Except(string message,
                        ExceptionType type = ExceptionType.SyntaxError,
                        Span? span = null,
                        ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span ?? Current.Span, info ?? ExceptionInfo.ReParser);

    private Token Expect(TokenKind kind, string? message = null)
    {
        if (Current.Kind == kind)
            return Eat();

        message ??= EOF ? "Expression not expected to end yet" : $"Unexpected \"{Current.Value}\"";
        Except(message, info: !EOF ? ExceptionInfo.Parser : ExceptionInfo.ReParser);

        return new Token(CONSTS.NULL, kind, Current.Span);
    }

    private Expression? GetPrimary()
    {
        if (EOF)
            return null;

        switch (Current.Kind)
        {
            case TokenKind.Null:
                return new Literal(Eat(), NodeKind.Null);

            case TokenKind.Boolean:
                return new Literal(Eat(), NodeKind.Boolean);

            case TokenKind.Integer:
                return new Literal(Eat(), NodeKind.Integer);

            case TokenKind.Float:
                return new Literal(Eat(), NodeKind.Float);

            case TokenKind.Char:
                return new Literal(Eat(), NodeKind.Char);

            case TokenKind.String:
                return new Literal(Eat(), NodeKind.String);

            case TokenKind.Identifier:
                return new Name(Eat(), NodeKind.Name);

            case TokenKind.OpenParenthesis:
                return GetParenthesized();

            default:
                Except($"Invalid syntax '{Current.Value}'", info:ExceptionInfo.Parser);
                Eat();
                return GetPrimary();
        }
    }

    private ParenExpression? GetParenthesized()
    {
        var openParen = Eat();
        var expression = Current.Kind != TokenKind.CloseParenthesis ? GetExpression() : null;
        var closeParen = Expect(TokenKind.CloseParenthesis, $"'(' was never closed");

        if (expression is null)
            Except($"Expression expected before close parenthesis");

        return new ParenExpression(openParen, expression, closeParen);
    }

    private Expression? GetSecondary(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = GetPrimary();
        else
        {
            var uOp = Eat();
            left = GetSecondary(unaryPrecedence);
            if (left is null)
            {
                Except($"Expected an expression after operator '{uOp.Value}'", ExceptionType.SyntaxError, uOp.Span);
                return null;
            }
            left = new UnaryExpression(uOp, left);
        }

        while (true)
        {
            var binaryPrecedence = Current.Kind.BinaryPrecedence();
            if (binaryPrecedence == 0 || binaryPrecedence <= parentPrecedence)
                break;

            var binOp = Eat();
            var right = GetSecondary(binaryPrecedence);

            if (right is null)
            {
                Except($"Expected an expression after operator '{binOp.Value}'", ExceptionType.SyntaxError, binOp.Span);
                return null;
            }

            left = new BinaryExpression(left!, binOp, right);
        }

        return left;
    }


    private Expression? GetAssignment()
    {
        var left = GetSecondary();

        if (Current.Kind.IsAssignment())
        {
            var eq   = Eat();
            var expr = GetExpression();

            if (left is not Name)
            {
                Except($"Invalid left-hand side assignee", ExceptionType.SyntaxError, left!.Span, ExceptionInfo.Parser);
                return left;
            }

            if (expr is null)
            {
                Except($"Expected an expression after equal", ExceptionType.SyntaxError, eq.Span);
                return null;
            }

            if (eq.Kind == TokenKind.Equal)
                return new AssignmentExpression((Name) left, eq, expr);

            return new CompoundAssignmentExpression((Name) left, eq, expr);
        }

        return left;
    }

    private Expression? GetExpression()
        => GetAssignment();

    private ExpressionStatement GetExpressionStatement()
        => new(GetExpression() ?? Literal.Unknown(Tokens[^1].Span));

    private BlockStatement GetBlockStatement()
    {
        List<Statement> block = new();

        var openBrace = Eat();

        while (Current.Kind != TokenKind.CloseCurlyBracket && !EOF)
            block.Add(GetStatement());

        var closeBrace = Expect(TokenKind.CloseCurlyBracket, "'{' was never closed");

        return new(openBrace, block.ToArray(), closeBrace);
    }

    private Statement GetStatement()
    {
        switch (Current.Kind)
        {
            case TokenKind.OpenCurlyBracket:
                return GetBlockStatement();
            default:
                return GetExpressionStatement();
        }
    }
}
