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
                        bool rereadLine = true)
        => Diagnostics.Add(type, message, span ?? Current.Span, rereadLine);

    private Token Expect(TokenKind kind, string? message = null, bool eatAnyway = false, bool reread = true, Span? span = null)
    {
        if (Current.Kind == kind)
            return Eat();

        message ??= EOF ? "Expression not expected to end yet" : $"Unexpected '{Current.Value}'";
        Except(message, span:span, rereadLine: !EOF || reread);

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
                return new NameLiteral(Eat());

            case TokenKind.OpenParenthesis:
                return GetParenthesized();

            case TokenKind.OpenSquareBracket:
                return GetList();

            default:
                Except($"Invalid syntax '{Current.Value}'", rereadLine:false);
                Eat();
                return GetPrimary();
        }
    }

    private ParenthesizedExpression GetParenthesized()
    {
        var openParen  = Eat();
        var expression = Current.Kind != TokenKind.CloseParenthesis
                       ? GetRange()
                       : null;
        var closeParen = Expect(TokenKind.CloseParenthesis, $"'(' was never closed");

        if (expression is null)
            Except($"Expression expected before close parenthesis",
                   span:new(openParen.Span, closeParen.Span),
                   rereadLine:false);

        return new(openParen, expression, closeParen);
    }

    private Expression[] GetSeparated(TokenKind endToken)
    {
        if (Current.Kind == endToken)
            return Array.Empty<Expression>();

        List<Expression> expressions = new();

        do
        {
            var expr = GetExpression();

            if (expr is not null)
            {
                expressions.Add(expr);

                if (IsNextKind(TokenKind.Comma))
                    continue;
            }
            break;
        }
        while (Current.Kind != endToken);

        return expressions.ToArray();
    }

    private ListLiteral GetList()
    {
        var openBracket  = Eat();
        var exprs        = Current.Kind != TokenKind.CloseSquareBracket
                         ? GetSeparated(TokenKind.CloseSquareBracket)
                         : Array.Empty<Expression>();
        var closeBracket = Expect(TokenKind.CloseSquareBracket, $"'[' was never closed");

        return new(openBracket, exprs, closeBracket);
    }

    private Expression? GetRange()
    {
        var start = GetExpression();

        if (Current.Kind == TokenKind.Colon && start is not null)
        {
            Expression? end, step = null;
            var colon1 = Eat();
            end = GetExpression();

            if (end is null)
            {
                Except("An end expression expected for range", span:colon1.Span);
                return Literal.Unknown(new(start.Span, colon1.Span));
            }

            if (IsNextKind(TokenKind.Colon))
                step = GetExpression();

            start = new RangeLiteral(start, end, step);
        }

        return start;
    }

    private Expression? GetIndexing(Expression iterable)
    {
        var openBracket  = Eat();
        var index        = Current.Kind != TokenKind.CloseSquareBracket
                         ? GetRange()
                         : null;
        var closeBracket = Expect(TokenKind.CloseSquareBracket, $"'[' was never closed");

        if (index is null)
        {
            Except($"Index expected before close bracket",
                   span:new(openBracket.Span, closeBracket.Span),
                   rereadLine:false);
            return Literal.Unknown(new(iterable.Span, closeBracket.Span));
        }

        return new IndexingExpression(iterable, openBracket, index, closeBracket);
    }

    private Expression? GetCounting(Expression? factor = null)
    {
        bool returnAfter = factor is not null;
        Expression? name = factor;
        Token       op;
    
        op   = Eat();

        if (!returnAfter)
            name = GetPrimary();

        if (name is null)
        {
            Except($"Expected a name before/after operator '{op.Value}'", span:op.Span);
            return Literal.Unknown(op.Span);
        }

        if (name is NameLiteral nm)
            return new CountingOperation(op, nm, returnAfter);

        Except($"Operand of '{op.Value}' must be a name", span:name.Span, rereadLine:false);
        return Literal.Unknown(new(op.Span, name.Span));
    }

    private Expression? GetIntermediate()
    {
        var factor = GetPrimary();

        while (Current.Kind == TokenKind.OpenSquareBracket)
            factor = GetIndexing(factor!);

        switch (Current.Kind)
        {
            case TokenKind.Increment:
            case TokenKind.Decrement:
                return GetCounting(factor!);

            default:
                return factor;
        }
    }

    private Expression? GetSecondary(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        // Get Unary
        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = GetIntermediate();
        else
        {
            if (Current.Kind.IsCounting())
                return GetCounting();

            var uOp = Eat();
            left = GetSecondary(unaryPrecedence);

            if (left is null)
            {
                Except($"Expected an expression after operator '{uOp.Value}'", span:uOp.Span);
                return Literal.Unknown(uOp.Span);
            }

            left = new UnaryOperation(uOp, left);
        }

        // Get Binary
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
                return Literal.Unknown(new(left!.Span, binOp.Span));
            }

            left = new BinaryOperation(left!, binOp, right);
        }

        // Get Ternary
        if (Current.Kind == TokenKind.QuestionMark && parentPrecedence == 0)
        {
            var mark      = Eat();

            var trueExpr  = GetSecondary();
            if (trueExpr is null)
            {
                Except($"Expected an expression after '{mark.Value}'", span:mark.Span);
                return Literal.Unknown(left!.Span);
            }

            var colon     = Expect(TokenKind.Colon, $"Expected a colon after if true expression");

            var falseExpr = GetSecondary();
            if (falseExpr is null)
            {
                Except($"Expected an expression after '{colon.Value}'", span:colon.Span);
                return trueExpr;
            }

            return new TernaryOperation(left!, trueExpr, falseExpr);
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

            if (left is not NameLiteral)
            {
                Except($"Invalid left-hand side assignee", span:left!.Span, rereadLine:false);
                return left;
            }

            if (expr is null)
            {
                Except($"Expected an expression after equal", span:eq.Span);
                return Literal.Unknown(new(left!.Span, eq.Span));
            }

            return new AssignmentExpression((NameLiteral) left, eq, expr);
        }

        return left;
    }

    private Expression? GetExpression()
        => GetAssignment();

    private ExpressionStatement GetExpressionStatement()
        => new(GetExpression() ?? Literal.Unknown(Tokens[^1].Span));

    private TypeClause GetTypeClause()
    {
        var type = Expect(TokenKind.Type, "Expected a type after colon", true);
        var dimension = 0;

        while (IsNextKind(TokenKind.OpenSquareBracket) && IsNextKind(TokenKind.CloseSquareBracket))
            dimension++;

        return new(type, Peek(-1).Span, dimension);
    }

    private DeclarationStatement GetDeclarationStatement()
    {
        Expression? expr = null;
        TypeClause? type = null;
        var hash = Eat();
        var isConst = Current.Kind == TokenKind.Asterisk;
        if (isConst) Eat();

        var name = Expect(TokenKind.Identifier, "Expected a name to declare");

        if (IsNextKind(TokenKind.Colon))
            type = GetTypeClause();

        if (Current.Kind == TokenKind.Equal)
        {
            var eq = Eat();
            expr = GetExpression();
            if (expr is null)
                Except($"Expected an expression after equal", span:eq.Span);
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

        if (Current.Kind is not (TokenKind.If or TokenKind.While))
            Expect(TokenKind.Colon, $"Expected a colon after 'else'");

        var statement   = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected an else statement");

        return new(elseKeyword, statement);
    }

    private ForStatement GetForStatement()
    {
        var forKeyword = Eat();
        var variable   = Expect(TokenKind.Identifier, "Expected an identifier");
        Expect(TokenKind.InOperator);
        var iterable   = GetExpression() ?? Literal.Unknown(Current.Span);
        Expect(TokenKind.Colon, $"Expected a colon after 'for' clause", span:forKeyword.Span);
        var statement = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected a statement", span:forKeyword.Span);
    
        return new(forKeyword, new(variable), iterable, statement);
    }

    private WhileStatement GetWhileStatement()
    {
        ElseClause? elseClause = null;
        var whileKeyword = Eat();
        var condition    = GetExpression() ?? Literal.Unknown(Current.Span);

        if (condition.Kind is NodeKind.Unknown)
                Except($"Expected an expression after 'while' keyword", span:whileKeyword.Span);

        Expect(TokenKind.Colon, $"Expected a colon after 'while' condition", span:whileKeyword.Span);
        var statement = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected a statement", span:whileKeyword.Span);

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(whileKeyword, condition, statement, elseClause);
    }

    private IfStatement GetIfStatement()
    {
        ElseClause? elseClause = null;
        var ifKeyword = Eat();
        var condition = GetExpression() ?? Literal.Unknown(Current.Span);

        if (condition.Kind is NodeKind.Unknown)
                Except($"Expected an expression after 'if' keyword", span:ifKeyword.Span);

        Expect(TokenKind.Colon, $"Expected a colon after 'if' condition", span:ifKeyword.Span);
        var statement = GetStatement();

        if (statement is ExpressionStatement exprStmt)
            if (exprStmt.Expression.Kind is NodeKind.Unknown)
                Except("Expected a statement", span:ifKeyword.Span);

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

            case TokenKind.While:
                return GetWhileStatement();

            case TokenKind.For:
                return GetForStatement();

            default:
                return GetExpressionStatement();
        }
    }
}
