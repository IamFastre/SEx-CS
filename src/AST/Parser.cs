using SEx.Lex;
using SEx.AST;
using SEx.Diagnose;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using System.Collections.Immutable;

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
        {
            var start = Current;
            var statement = GetStatement();

            stmts.Add(statement);

            if (start == Current)
                Eat();
        }

        return Tree = new(stmts.ToArray());
    }

    private Token Eat()
    {
        var current = Current;
        Index++;
        return current;
    }

    private Token Expect(TokenKind kind, bool eatAnyway = false, Span? span = null, bool rereadLine = false)
    {
        if (Current.Kind == kind)
            return Eat();

        Diagnostics.Report.ExpectedToken(kind.ToString(), Current.Value, span ?? Current.Span, rereadLine);

        if (eatAnyway && !EOF)
            Eat();

        return new(CONSTS.EMPTY, kind, Current.Span);
    }

    private Expression FabricateExpression(Span? span = null)
        => Expression.Unknown(span ?? Current.Span);

    private bool IsNextKind(TokenKind kind)
        => IsNextKind(kind, out _);

    private bool IsNextKind(TokenKind kind, out Token? token)
        => (token = Optional(kind))?.Kind == kind;

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
                Diagnostics.Report.InvalidSyntax(Current.Value, Current.Span);
                return FabricateExpression();
        }
    }

    private Expression GetParenthesized()
    {
        // TODO: Fix this massive pile of shit
        Node?  node  = null;
        Token? arrow = null;

        var openParen = Eat();

        if (Current.Kind == TokenKind.Identifier && Peek().Kind == TokenKind.Colon)
            node = GetSeparated(GetParameterClause, TokenKind.CloseParenthesis);
        else if (Current.Kind != TokenKind.CloseParenthesis)
            node = GetRange();

        var closeParen = Expect(TokenKind.CloseParenthesis);

        if (node is Expression expr)
            return new ParenthesizedExpression(openParen, expr, closeParen);

        if (node is SeparatedClause<ParameterClause> pc)
        {
            var hint  = IsNextKind(TokenKind.DashArrow) ? GetTypeClause() : null;
            arrow = Expect(TokenKind.EqualArrow);
            var stmt  = GetFunctionBodyStatement();

            return new FunctionLiteral(openParen, pc, closeParen, hint, arrow, stmt);
        }

        if (IsNextKind(TokenKind.EqualArrow, out arrow) || IsNextKind(TokenKind.DashArrow))
        {
            var hint  = Peek(-1).Kind == TokenKind.DashArrow ? GetTypeClause() : null;
            arrow ??= Expect(TokenKind.EqualArrow);
            var stmt  = GetFunctionBodyStatement();
            return new FunctionLiteral(openParen, SeparatedClause<ParameterClause>.Empty, closeParen, hint, arrow, stmt);
        }

        if (node is null)
            Diagnostics.Report.ExpressionExpectedAfter(openParen.Value, openParen.Span);

        return new ParenthesizedExpression(openParen, FabricateExpression(new(openParen.Span, closeParen.Span)), closeParen);
    }

    private SeparatedClause<Expression> GetSeparated(TokenKind endToken)
        => GetSeparated(GetExpression, endToken);

    private SeparatedClause<T> GetSeparated<T>(Func<T?> func, TokenKind endToken) where T : Node
    {
        if (Current.Kind == endToken)
            return SeparatedClause<T>.Empty;

        var expressions = ImmutableArray.CreateBuilder<T>();
        var separators  = ImmutableArray.CreateBuilder<Token>();

        do
        {
            var expr = func();

            if (expr is not null)
            {
                expressions.Add(expr);

                if (Current.Kind == TokenKind.Comma)
                {
                    separators.Add(Eat());
                    continue;
                }
            }
            break;
        }
        while (Current.Kind != endToken);

        return new(expressions.ToArray(), separators.ToArray());
    }

    private ListLiteral GetList()
    {
        var openBracket  = Eat();
        var exprs        = GetSeparated(TokenKind.CloseSquareBracket);
        var closeBracket = Expect(TokenKind.CloseSquareBracket);

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
                Diagnostics.Report.ExpressionExpectedAfter(colon1.Value, colon1.Span);
                return FabricateExpression(new(start.Span, colon1.Span));
            }

            if (IsNextKind(TokenKind.Colon))
                step = GetExpression();

            start = new RangeLiteral(start, end, step);
        }

        return start;
    }

    private Expression? GetCall(Expression func)
    {
        var openParen  = Eat();
        var args       = GetSeparated(TokenKind.CloseParenthesis);
        var closeParen = Expect(TokenKind.CloseParenthesis);

        return new CallExpression(func, openParen, args, closeParen);
    }

    private Expression? GetIndexing(Expression iterable)
    {
        var openBracket  = Eat();
        var index        = Current.Kind != TokenKind.CloseSquareBracket
                         ? GetRange()
                         : null;
        var closeBracket = Expect(TokenKind.CloseSquareBracket);

        if (index is null)
        {
            Diagnostics.Report.ExpressionExpectedAfter(openBracket.Value, openBracket.Span);
            return FabricateExpression(new(iterable.Span, closeBracket.IsFabricated ? openBracket.Span : closeBracket.Span));
        }

        return new IndexingExpression(iterable, openBracket, index, closeBracket);
    }

    private Expression? GetCounting(Expression? factor = null)
    {
        bool returnAfter = factor is not null;
        Expression? name = factor;
        Token       op;
    
        op = Eat();

        if (!returnAfter)
            name = GetExpression();

        if (name is null)
        {
            Diagnostics.Report.NameExpected(op.Value, op.Span);
            return FabricateExpression(op.Span);
        }

        if (name is NameLiteral nm)
            return new CountingOperation(op, nm, returnAfter);

        Diagnostics.Report.OperandMustBeName(op.Value, name.Span);
        return FabricateExpression(new(op.Span, name.Span));
    }

    private Expression? GetIntermediate()
    {
        var factor = GetPrimary();

        // Indexing
        while (Current.Kind == TokenKind.OpenSquareBracket)
            factor = GetIndexing(factor!);

        // Calling
        if (Current.Kind == TokenKind.OpenParenthesis)
            factor = GetCall(factor!);

        switch (Current.Kind)
        {
            case TokenKind.Increment:
            case TokenKind.Decrement:
                return GetCounting(factor!);

            default:
                return factor;
        }
    }

    private Expression? GetConversion()
    {
        var expr = GetIntermediate();

        while (IsNextKind(TokenKind.DashArrow) && expr is not null)
            expr = new ConversionExpression(expr, GetTypeClause());

        return expr;
    }

    private Expression? GetSecondary(int parentPrecedence = 0)
    {
        Expression? left;
        var unaryPrecedence = Current.Kind.UnaryPrecedence();

        // Get Unary
        if (unaryPrecedence == 0 || unaryPrecedence < parentPrecedence)
            left = GetConversion();
        else
        {
            if (Current.Kind.IsCounting())
                return GetCounting();

            var uOp = Eat();
            left = GetSecondary(unaryPrecedence);

            if (left is null)
            {
                Diagnostics.Report.ExpressionExpectedAfter(uOp.Value, uOp.Span);
                return FabricateExpression(uOp.Span);
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
                Diagnostics.Report.ExpressionExpectedAfter(binOp.Value, binOp.Span);
                return FabricateExpression(new(left!.Span, binOp.Span));
            }

            left = new BinaryOperation(left!, binOp, right);
        }

        // Get Ternary
        if (Current.Kind == TokenKind.QuestionMark && parentPrecedence == 0)
        {
            var qMark     = Eat();
            var trueExpr  = GetSecondary();

            if (trueExpr is null)
            {
                Diagnostics.Report.ExpressionExpectedAfter(qMark.Value, qMark.Span);
                return FabricateExpression(new(left!.Span, qMark.Span));
            }

            var colon     = Expect(TokenKind.Colon);

            var falseExpr = GetSecondary();
            if (falseExpr is null)
            {
                Diagnostics.Report.ExpressionExpectedAfter(colon.Value, colon.Span);
                return trueExpr;
            }

            return new TernaryOperation(left!, trueExpr, falseExpr);
        }

        return left;
    }

    private Expression? GetAssignment()
    {
        var left = GetSecondary();

        if (left is not null && Current.Kind.IsAssignment())
        {
            var eq   = Eat();
            var expr = GetExpression();

            if (left is not NameLiteral)
            {
                Diagnostics.Report.InvalidAssignee(left.Span);
                return left;
            }

            if (expr is null)
            {
                Diagnostics.Report.ExpressionExpectedAfter(eq.Value, eq.Span);
                return FabricateExpression(new(left!.Span, eq.Span));
            }

            return new AssignmentExpression((NameLiteral) left, eq, expr);
        }

        return left;
    }

    private Expression? GetExpression()
        => GetAssignment();

    private ExpressionStatement GetExpressionStatement()
    {
        var expr = GetExpression();

        if (expr is null)
            Diagnostics.Report.StatementExpected(Current.Span);

        return new(expr ?? FabricateExpression());
    }

    private GenericTypeClause GetGenericTypeClause(Token type)
    {
        List<TypeClause> types = new();
        var dimension = 0;

        do
        {
            if (Current.Kind == TokenKind.Comma)
                Eat();
            types.Add(GetTypeClause());
        }
        while (Current.Kind == TokenKind.Comma && Current.Kind != TokenKind.Greater);
        var close = Expect(TokenKind.Greater);

        while (IsNextKind(TokenKind.OpenSquareBracket) && IsNextKind(TokenKind.CloseSquareBracket))
            dimension++;

        return new(type, types.ToArray(), close.Span, dimension);
    }

    private TypeClause GetTypeClause()
    {
        var type      = Expect(TokenKind.Type);
        var dimension = 0;

        if (IsNextKind(TokenKind.Less))
            return GetGenericTypeClause(type);

        while (IsNextKind(TokenKind.OpenSquareBracket) && IsNextKind(TokenKind.CloseSquareBracket))
            dimension++;

        return new(type, Peek(-1).Span, dimension);
    }

    private BlockStatement GetBlockStatement(bool inFunction = false)
    {
        var block     = ImmutableArray.CreateBuilder<Statement>();
        var openBrace = Eat();

        while (Current.Kind != TokenKind.CloseCurlyBracket && !EOF)
            block.Add(GetStatement(inFunction));

        var closeBrace = Expect(TokenKind.CloseCurlyBracket, rereadLine:true);

        return new(openBrace, block.ToArray(), closeBrace);
    }

    private DeclarationStatement GetDeclarationStatement()
    {
        Expression? expr = null;
        TypeClause? type = null;
        var hash    = Eat();
        var isConst = IsNextKind(TokenKind.Asterisk);

        var name = Expect(TokenKind.Identifier);

        if (IsNextKind(TokenKind.Colon))
            type = GetTypeClause();

        var equal = isConst ? Optional(TokenKind.Equal) : Expect(TokenKind.Equal);
        if (equal is not null)
        {
            expr  = GetExpression();
            if (expr is null && !equal.IsFabricated)
                Diagnostics.Report.ExpressionExpectedAfter(equal.Value, equal.Span);
        }
    
        return new(hash, isConst, new(name), expr ?? FabricateExpression(), type);
    }

    private FunctionStatement GetFunctionStatement()
    {
        TypeClause? type = null;
        var funcSymbol   = Eat();
        var isConst      = IsNextKind(TokenKind.Asterisk);
        var name         = Expect(TokenKind.Identifier);

        Expect(TokenKind.OpenParenthesis);
        var parameters   = GetSeparated(GetParameterClause, TokenKind.CloseParenthesis);
        Expect(TokenKind.CloseParenthesis);

        if (IsNextKind(TokenKind.DashArrow))
            type = GetTypeClause();

        Expect(TokenKind.Colon);
        var statement    = GetFunctionBodyStatement();

        return new(funcSymbol, isConst, new(name), type, parameters, statement);
    }

    private ParameterClause? GetParameterClause()
    {
        var name  = Expect(TokenKind.Identifier);

        var colon = Expect(TokenKind.Colon);
        if (colon.IsFabricated)
            return null;

        var type  = GetTypeClause();

        return new(new(name), type);
    }

    private Statement GetFunctionBodyStatement()
    {
        if (Current.Kind == TokenKind.OpenCurlyBracket)
            return GetBlockStatement(inFunction:true);

        return GetExpressionStatement();
    }

    private IfStatement GetIfStatement()
    {
        ElseClause? elseClause = null;
        var ifKeyword = Eat();
        var condition = GetExpression() ?? FabricateExpression();

        if (condition.Kind is NodeKind.Unknown)
                Diagnostics.Report.ExpressionExpectedAfter(ifKeyword.Value, ifKeyword.Span);

        Expect(TokenKind.Colon, span:ifKeyword.Span);
        var statement = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(ifKeyword, condition, statement, elseClause);
    }

    private WhileStatement GetWhileStatement()
    {
        ElseClause? elseClause = null;
        var whileKeyword = Eat();
        var condition    = GetExpression() ?? FabricateExpression();

        if (condition.Kind is NodeKind.Unknown)
                Diagnostics.Report.ExpressionExpectedAfter(whileKeyword.Value, whileKeyword.Span);

        Expect(TokenKind.Colon, span:whileKeyword.Span);
        var statement = GetStatement();

        if (Current.Kind == TokenKind.Else)
            elseClause = GetElseClause();

        return new(whileKeyword, condition, statement, elseClause);
    }

    private ElseClause GetElseClause()
    {
        var elseKeyword = Eat();

        if (Current.Kind is not (TokenKind.If or TokenKind.While))
            Expect(TokenKind.Colon);

        var statement   = GetStatement();

        return new(elseKeyword, statement);
    }

    private ForStatement GetForStatement()
    {
        var forKeyword = Eat();
        var variable   = Expect(TokenKind.Identifier);
        Expect(TokenKind.InOperator);
        var iterable   = GetExpression() ?? FabricateExpression();
        Expect(TokenKind.Colon, span:forKeyword.Span);
        var statement  = GetStatement();

        return new(forKeyword, new(variable), iterable, statement);
    }

    private ReturnStatement GetReturnStatement()
    {
        var returnKeyword = Eat();
        var expr = GetExpression() ?? FabricateExpression();

        return new(returnKeyword, expr);
    }

    private Statement GetStatement(bool inFunction = false)
    {
        switch (Current.Kind)
        {
            case TokenKind.Hash:
                return GetDeclarationStatement();

            case TokenKind.OpenCurlyBracket:
                return GetBlockStatement();

            case TokenKind.FunctionSymbol:
                return GetFunctionStatement();

            case TokenKind.If:
                return GetIfStatement();

            case TokenKind.While:
                return GetWhileStatement();

            case TokenKind.For:
                return GetForStatement();

            case TokenKind.Return when inFunction:
                return GetReturnStatement();

            default:
                return GetExpressionStatement();
        }
    }
}
