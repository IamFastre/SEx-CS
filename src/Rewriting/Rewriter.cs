using System.Collections.Immutable;
using SEx.SemanticAnalysis;

namespace SEx.Rewriting;

public abstract class Rewriter
{
    public SemanticProgramStatement Rewrite(SemanticProgramStatement program)
        => new(program.Body.Select(RewriteStatement).ToArray());

    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */
    public SemanticStatement RewriteStatement(SemanticStatement stmt) => stmt.Kind switch
    {
        SemanticKind.BlockStatement       => RewriteBlockStatement((SemanticBlockStatement) stmt),
        SemanticKind.ExpressionStatement  => RewriteExpressionStatement((SemanticExpressionStatement) stmt),
        SemanticKind.DeclarationStatement => RewriteDeclarationStatement((SemanticDeclarationStatement) stmt),
        SemanticKind.IfStatement          => RewriteIfStatement((SemanticIfStatement) stmt),
        SemanticKind.WhileStatement       => RewriteWhileStatement((SemanticWhileStatement) stmt),
        SemanticKind.ForStatement         => RewriteForStatement((SemanticForStatement) stmt),
        SemanticKind.FunctionStatement    => RewriteFunctionStatement((SemanticFunctionStatement) stmt),
        SemanticKind.ReturnStatement      => RewriteReturnStatement((SemanticReturnStatement) stmt),

        _ => throw new Exception("Statement unadded")
    };

    protected virtual SemanticBlockStatement RewriteBlockStatement(SemanticBlockStatement bs)
    {
        var same  = true;
        var stmts = ImmutableArray.CreateBuilder<SemanticStatement>();
        foreach (var stmt in bs.Body)
        {
            var reStmt = RewriteStatement(stmt);
            same = same && reStmt == stmt;
            stmts.Add(stmt);
        }

        if (same)
            return bs;

        return new(stmts, bs.Span);
    }

    protected virtual SemanticExpressionStatement RewriteExpressionStatement(SemanticExpressionStatement es)
    {
        var expr = RewriteExpression(es.Expression);
        if (expr == es.Expression)
            return es;

        return new(expr);
    }

    protected virtual SemanticDeclarationStatement RewriteDeclarationStatement(SemanticDeclarationStatement ds)
    {
        var expr = RewriteExpression(ds.Expression);
        if (expr == ds.Expression)
            return ds;

        return new(ds.Variable, expr, ds.Span);
    }

    protected virtual SemanticIfStatement RewriteIfStatement(SemanticIfStatement @is)
    {
        var condition = RewriteExpression(@is.Condition);
        var thenStmt  = RewriteStatement(@is.Then);
        var elseStmt  = @is.ElseStatement is null ? null : RewriteStatement(@is.ElseStatement);
        if (condition == @is.Condition && thenStmt == @is.Then && elseStmt == @is.ElseStatement)
            return @is;

        return new(condition, thenStmt, elseStmt, @is.Span);
    }

    protected virtual SemanticWhileStatement RewriteWhileStatement(SemanticWhileStatement ws)
    {
        var condition = RewriteExpression(ws.Condition);
        var thenStmt  = RewriteStatement(ws.Body);
        var elseStmt  = ws.ElseStatement is null ? null : RewriteStatement(ws.ElseStatement);
        if (condition == ws.Condition && thenStmt == ws.Body && elseStmt == ws.ElseStatement)
            return ws;

        return new(condition, thenStmt, elseStmt, ws.Span);
    }

    protected virtual SemanticForStatement RewriteForStatement(SemanticForStatement fs)
    {
        var expr = RewriteExpression(fs.Iterable);
        var body = RewriteStatement(fs.Body);
        if (expr == fs.Iterable && body == fs.Body)
            return fs;

        return new(fs.Variable, expr, body, fs.Span);
    }

    protected virtual SemanticFunctionStatement RewriteFunctionStatement(SemanticFunctionStatement fs)
    {
        var body = RewriteStatement(fs.Body);
        if (body == fs.Body)
            return fs;

        return new(fs.Function, fs.Parameters, fs.ReturnType, body, fs.Span);
    }

    protected virtual SemanticReturnStatement RewriteReturnStatement(SemanticReturnStatement rs)
    {
        var expr = rs.Expression is null ? null : RewriteExpression(rs.Expression);
        if (expr == rs.Expression)
            return rs;

        return new(expr, rs.Span);
    }


    /* ====================================================================== */
    /*                              Expressions                               */
    /* ====================================================================== */
    public SemanticExpression RewriteExpression(SemanticExpression expr) => expr.Kind switch
    {
        SemanticKind.FailedExpression      => RewriteFailedExpression((SemanticFailedExpression) expr),
        SemanticKind.AssignExpression      => RewriteAssignment((SemanticAssignment) expr),
        SemanticKind.IndexAssignExpression => RewriteIndexAssignment((SemanticIndexAssignment) expr),
        SemanticKind.CallExpression        => RewriteCallExpression((SemanticCallExpression) expr),
        SemanticKind.IndexingExpression    => RewriteIndexingExpression((SemanticIndexingExpression) expr),
        SemanticKind.FailedOperation       => RewriteFailedOperation((SemanticFailedOperation) expr),
        SemanticKind.UnaryOperation        => RewriteUnaryOperation((SemanticUnaryOperation) expr),
        SemanticKind.CountingOperation     => RewriteCountingOperation((SemanticCountingOperation) expr),
        SemanticKind.BinaryOperation       => RewriteBinaryOperation((SemanticBinaryOperation) expr),
        SemanticKind.TernaryOperation      => RewriteTernaryOperation((SemanticTernaryOperation) expr),
        SemanticKind.ConversionOperation   => RewriteConversionOperation((SemanticConversionOperation) expr),

        _ => throw new Exception("Expression unadded")
    };

    protected virtual SemanticFailedExpression RewriteFailedExpression(SemanticFailedExpression fe)
        => fe;

    protected virtual SemanticAssignment RewriteAssignment(SemanticAssignment ae)
    {
        var expr = RewriteExpression(ae.Expression);
        if (expr == ae.Expression)
            return ae;
        
        return new(ae.Assignee, expr, ae.Operator, ae.Span);
    }

    protected virtual SemanticIndexAssignment RewriteIndexAssignment(SemanticIndexAssignment iae)
    {    
        var indx = (SemanticIndexingExpression) RewriteExpression(iae.Indexing);
        var expr = RewriteExpression(iae.Expression);
        if (indx == iae.Indexing && expr == iae.Expression)
            return iae;

        return new(indx, expr, iae.Span);
    }

    protected virtual SemanticCallExpression RewriteCallExpression(SemanticCallExpression ce)
    {
        var func = RewriteExpression(ce.Function);
        var same = func == ce.Function;
        var args = ImmutableArray.CreateBuilder<SemanticExpression>();
        foreach (var arg in ce.Arguments)
        {
            var reArg = RewriteExpression(arg);
            same = same && reArg == arg;
            args.Add(arg);
        }

        if (same)
            return ce;

        return new(func, ce.Type, args, ce.Span);
    }

    protected virtual SemanticIndexingExpression RewriteIndexingExpression(SemanticIndexingExpression ie)
    {
        var expr = RewriteExpression(ie.Iterable);
        var indx = RewriteExpression(ie.Index);
        if (expr == ie.Iterable && indx == ie.Index)
            return ie;

        return new(expr, indx, ie.Type, ie.Span);
    }

    protected virtual SemanticFailedOperation RewriteFailedOperation(SemanticFailedOperation fop)
    {
        var same  = true;
        var exprs = ImmutableArray.CreateBuilder<SemanticExpression>();
        foreach (var expr in fop.Expressions)
        {
            var reArg = RewriteExpression(expr);
            same = same && reArg == expr;
            exprs.Add(expr);
        }

        if (same)
            return fop;

        return new(exprs, fop.Span);
    }

    protected virtual SemanticUnaryOperation RewriteUnaryOperation(SemanticUnaryOperation uop)
    {
        var oprd = RewriteExpression(uop.Operand);
        if (oprd == uop.Operand)
            return uop;
        
        return new(oprd, uop.OperationKind, uop.Span);
    }

    protected virtual SemanticCountingOperation RewriteCountingOperation(SemanticCountingOperation cop)
    {
        var name = (SemanticName) RewriteExpression(cop.Name);
        if (name == cop.Name)
            return cop;
        
        return new(name, cop.OperationKind, cop.Span);
    }

    protected virtual SemanticBinaryOperation RewriteBinaryOperation(SemanticBinaryOperation biop)
    {
        var right = RewriteExpression(biop.Right);
        var left  = RewriteExpression(biop.Left);
        if (right == biop.Right && left == biop.Left)
            return biop;

        return new(right, biop.Operator, left, biop.Span);
    }

    protected virtual SemanticTernaryOperation RewriteTernaryOperation(SemanticTernaryOperation terop)
    {
        var condition = RewriteExpression(terop.Condition);
        var trueExpr  = RewriteExpression(terop.TrueExpression);
        var falseExpr = RewriteExpression(terop.FalseExpression);
        if (condition == terop.Condition && trueExpr == terop.TrueExpression && falseExpr == terop.FalseExpression)
            return terop;

        return new(condition, trueExpr, falseExpr, terop.Span);
    }

    protected virtual SemanticConversionOperation RewriteConversionOperation(SemanticConversionOperation cop)
    {
        var expr = RewriteExpression(cop.Expression);
        if (expr == cop.Expression)
            return cop;

        return new(expr, cop.Target, cop.ConversionKind, cop.Span);
    }


    /* ====================================================================== */
    /*                                Literals                                */
    /* ====================================================================== */
    // public SemanticExpression RewriteLiteral(SemanticExpression litr) => litr.Kind switch
    // {
    //     SemanticKind.Literal       => litr,
    //     SemanticKind.FormatString  => RewriteFormatString(litr),
    //     SemanticKind.Range         => RewriteRange(litr),
    //     SemanticKind.List          => RewriteList(litr),
    //     SemanticKind.Name          => RewriteName(litr),
    //     SemanticKind.Function      => RewriteFunction(litr),

    //     _ => throw new Exception("Literal unadded")
    // };

    // private SemanticExpression RewriteFormatString(SemanticExpression litr) { }

    // private SemanticExpression RewriteRange(SemanticExpression litr) { }

    // private SemanticExpression RewriteList(SemanticExpression litr) { }

    // private SemanticExpression RewriteName(SemanticExpression litr) { }

    // private SemanticExpression RewriteFunction(SemanticExpression litr) { }
}
