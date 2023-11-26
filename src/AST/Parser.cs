using SEx.Lex;
using SEx.AST;
using SEx.Diagnose;
using SEx.Generic.Constants;
using SEx.Generic.Text;

namespace SEx.Parse;

internal class Parser
{
    public List<Token>       Tokens      { get; }
    public Diagnostics       Diagnostics { get; }
    public ProgramStatement? Tree        { get; protected set; }

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

    public ProgramStatement Parse()
    {
        var stmts = new List<Statement>();
        while (!EOF)
            stmts.Add(GetStatement() ?? ExpressionStatement.Empty(Tokens[^1].Span));

        return Tree = new(stmts.ToArray());
    }

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

    private Token Expect(TokenKind kind, string? message = null, bool eatAnyway = false)
    {
        if (Current.Kind == kind)
            return Eat();

        message ??= EOF ? "Expression not expected to end yet" : $"Unexpected \"{Current.Value}\"";
        Except(message, info: !EOF ? ExceptionInfo.Parser : ExceptionInfo.ReParser);

        if (eatAnyway && !EOF)
            Eat();

        return new Token(CONSTS.VOID, kind, Current.Span);
    }

    private bool IsNextKind(TokenKind kind)
        => Optional(kind)?.Kind == kind;

    private Token? Optional(TokenKind kind)
    {
        if (Current.Kind == kind)
            return Eat();

        return null;
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
                return new Name(Eat());

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
            Except($"Expression expected before close parenthesis", info:ExceptionInfo.Parser);

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
                Except($"Expected an expression after operator '{uOp.Value}'", span:uOp.Span);
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
                Except($"Expected an expression after operator '{binOp.Value}'", span:binOp.Span);
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
                Except($"Invalid left-hand side assignee", span:left!.Span, info:ExceptionInfo.Parser);
                return left;
            }

            if (expr is null)
            {
                Except($"Expected an expression after equal", span:eq.Span);
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

    private DeclarationStatement GetDeclarationStatement()
    {
        Expression? expr = null;
        Token? type = null;
        var hash = Eat();
        var isConst = Current.Kind == TokenKind.Asterisk;
        if (isConst) Eat();

        var name = Expect(TokenKind.Identifier, "Expected a name to declare");

        if (IsNextKind(TokenKind.Colon))
            type = Expect(TokenKind.Type, "Expected a type after colon", true);

        if (IsNextKind(TokenKind.Equal))
        {
            expr = GetExpression();
            if (expr is null)
                Except($"Expected an expression after equal", span:new(hash.Span, Current.Span));
        }

        return new(hash, new(name), type, expr, isConst);
    }

    private BlockStatement GetBlockStatement()
    {
        List<Statement> block = new();

        var openBrace = Eat();

        while (Current.Kind != TokenKind.CloseCurlyBracket && !EOF)
            block.Add(GetStatement());

        var closeBrace = Expect(TokenKind.CloseCurlyBracket, "'{' was never closed");

        return new(openBrace, block.ToArray(), closeBrace);
    }

    private ElseClause GetElseClause()
    {
        var elseKeyword = Eat();

        Expect(TokenKind.Colon, $"Expected a colon after if condition");
        var statement   = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected an else statement");

        return new(elseKeyword, statement);
    }

    private IfStatement GetIfStatement()
    {
        ElseClause? elseClause = null;
        var ifKeyword = Eat();
        var condition = GetExpression() ?? Literal.Unknown(Current.Span);

        if (condition.Kind is NodeKind.Unknown)
                Except($"Expected an expression after if keyword", span:ifKeyword.Span);

        Expect(TokenKind.Colon, $"Expected a colon after if condition");
        var statement = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected an if statement");


        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(ifKeyword, condition, statement, elseClause);
    }

    private Statement GetStatement()
    {
        switch (Current.Kind)
        {
            case TokenKind.Hash:
                return GetDeclarationStatement();
            case TokenKind.OpenCurlyBracket:
                return GetBlockStatement();
            case TokenKind.If:
                return GetIfStatement();
            default:
                return GetExpressionStatement();
        }
    }
}
