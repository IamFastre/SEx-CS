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
        SemanticKind.ExpressionStatement  => RewriteExpressionStatement((SemanticExpressionStatement) stmt),
        SemanticKind.BlockStatement       => RewriteBlockStatement((SemanticBlockStatement) stmt),
        SemanticKind.DeclarationStatement => RewriteDeclarationStatement((SemanticDeclarationStatement) stmt),
        SemanticKind.IfStatement          => RewriteIfStatement((SemanticIfStatement) stmt),
        SemanticKind.WhileStatement       => RewriteWhileStatement((SemanticWhileStatement) stmt),
        SemanticKind.ForStatement         => RewriteForStatement((SemanticForStatement) stmt),
        SemanticKind.FunctionStatement    => RewriteFunctionStatement((SemanticFunctionStatement) stmt),
        SemanticKind.ReturnStatement      => RewriteReturnStatement((SemanticReturnStatement) stmt),

        _ => throw new Exception("Statement unadded")
    };

    protected virtual SemanticStatement RewriteExpressionStatement(SemanticExpressionStatement es)
    {
        var expr = RewriteExpression(es.Expression);
        if (expr == es.Expression)
            return es;

        return new SemanticExpressionStatement(expr);
    }

    protected virtual SemanticStatement RewriteBlockStatement(SemanticBlockStatement bs)
    {
        var same  = true;
        var stmts = ImmutableArray.CreateBuilder<SemanticStatement>();
        foreach (var stmt in bs.Body)
        {
            var reStmt = RewriteStatement(stmt);
            same &= reStmt == stmt;
            stmts.Add(stmt);
        }

        if (same)
            return bs;

        return new SemanticBlockStatement(stmts, bs.Span);
    }

    protected virtual SemanticStatement RewriteDeclarationStatement(SemanticDeclarationStatement ds)
    {
        var expr = RewriteExpression(ds.Expression);
        if (expr == ds.Expression)
            return ds;

        return new SemanticDeclarationStatement(ds.Variable, expr, ds.Span);
    }

    protected virtual SemanticStatement RewriteIfStatement(SemanticIfStatement @is)
    {
        var condition = RewriteExpression(@is.Condition);
        var thenStmt  = RewriteStatement(@is.Then);
        var elseStmt  = @is.ElseStatement is null ? null : RewriteStatement(@is.ElseStatement);
        if (condition == @is.Condition && thenStmt == @is.Then && elseStmt == @is.ElseStatement)
            return @is;

        return new SemanticIfStatement(condition, thenStmt, elseStmt, @is.Span);
    }

    protected virtual SemanticStatement RewriteWhileStatement(SemanticWhileStatement ws)
    {
        var condition = RewriteExpression(ws.Condition);
        var thenStmt  = RewriteStatement(ws.Body);
        var elseStmt  = ws.ElseStatement is null ? null : RewriteStatement(ws.ElseStatement);
        if (condition == ws.Condition && thenStmt == ws.Body && elseStmt == ws.ElseStatement)
            return ws;

        return new SemanticWhileStatement(condition, thenStmt, elseStmt, ws.Span);
    }

    protected virtual SemanticStatement RewriteForStatement(SemanticForStatement fs)
    {
        var expr = RewriteExpression(fs.Iterable);
        var body = RewriteStatement(fs.Body);
        if (expr == fs.Iterable && body == fs.Body)
            return fs;

        return new SemanticForStatement(fs.Variable, fs.VarSpan, expr, body, fs.Span);
    }

    protected virtual SemanticStatement RewriteFunctionStatement(SemanticFunctionStatement fs)
    {
        var body = RewriteStatement(fs.Body);
        if (body == fs.Body)
            return fs;

        return new SemanticFunctionStatement(fs.Function, fs.Parameters, fs.ReturnType, body, fs.Span);
    }

    protected virtual SemanticStatement RewriteReturnStatement(SemanticReturnStatement rs)
    {
        var expr = rs.Expression is null ? null : RewriteExpression(rs.Expression);
        if (expr == rs.Expression)
            return rs;

        return new SemanticReturnStatement(expr, rs.Span);
    }


    /* ====================================================================== */
    /*                              Expressions                               */
    /* ====================================================================== */
    public SemanticExpression RewriteExpression(SemanticExpression expr) => expr.Kind switch
    {
        // Literals
        SemanticKind.Literal               => RewriteLiteral((SemanticLiteral) expr),
        SemanticKind.FormatString          => RewriteFormatString((SemanticFormatString) expr),
        SemanticKind.Range                 => RewriteRange((SemanticRange) expr),
        SemanticKind.List                  => RewriteList((SemanticList) expr),
        SemanticKind.Name                  => RewriteName((SemanticName) expr),
        SemanticKind.Function              => RewriteFunction((SemanticFunction) expr),
        // Expressions
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

    protected virtual SemanticExpression RewriteLiteral(SemanticLiteral l)
        => l;

    protected virtual SemanticExpression RewriteFormatString(SemanticFormatString fsl)
        => fsl;

    protected virtual SemanticExpression RewriteRange(SemanticRange rl)
        => rl;

    protected virtual SemanticExpression RewriteList(SemanticList ll)
        => ll;

    protected virtual SemanticExpression RewriteName(SemanticName nl)
        => nl;

    protected virtual SemanticExpression RewriteFunction(SemanticFunction fl)
        => fl;

    protected virtual SemanticExpression RewriteFailedExpression(SemanticFailedExpression fe)
        => fe;

    protected virtual SemanticExpression RewriteAssignment(SemanticAssignment ae)
    {
        var expr = RewriteExpression(ae.Expression);
        if (expr == ae.Expression)
            return ae;
        
        return new SemanticAssignment(ae.Assignee, expr, ae.Operator, ae.Span);
    }

    protected virtual SemanticExpression RewriteIndexAssignment(SemanticIndexAssignment iae)
    {    
        var indx = (SemanticIndexingExpression) RewriteExpression(iae.Indexing);
        var expr = RewriteExpression(iae.Expression);
        if (indx == iae.Indexing && expr == iae.Expression)
            return iae;

        return new SemanticIndexAssignment(indx, expr, iae.Span);
    }

    protected virtual SemanticExpression RewriteCallExpression(SemanticCallExpression ce)
    {
        var func = RewriteExpression(ce.Function);
        var same = func == ce.Function;
        var args = ImmutableArray.CreateBuilder<SemanticExpression>();
        foreach (var arg in ce.Arguments)
        {
            var reArg = RewriteExpression(arg);
            same &= reArg == arg;
            args.Add(arg);
        }

        if (same)
            return ce;

        return new SemanticCallExpression(func, ce.Type, args, ce.Span);
    }

    protected virtual SemanticExpression RewriteIndexingExpression(SemanticIndexingExpression ie)
    {
        var expr = RewriteExpression(ie.Iterable);
        var indx = RewriteExpression(ie.Index);
        if (expr == ie.Iterable && indx == ie.Index)
            return ie;

        return new SemanticIndexingExpression(expr, indx, ie.Type, ie.Span);
    }

    protected virtual SemanticExpression RewriteFailedOperation(SemanticFailedOperation fop)
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

        return new SemanticFailedOperation(exprs, fop.Span);
    }

    protected virtual SemanticExpression RewriteUnaryOperation(SemanticUnaryOperation uop)
    {
        var oprd = RewriteExpression(uop.Operand);
        if (oprd == uop.Operand)
            return uop;
        
        return new SemanticUnaryOperation(oprd, uop.OperationKind, uop.Span);
    }

    protected virtual SemanticExpression RewriteCountingOperation(SemanticCountingOperation cop)
    {
        var name = (SemanticName) RewriteExpression(cop.Name);
        if (name == cop.Name)
            return cop;
        
        return new SemanticCountingOperation(name, cop.OperationKind, cop.Span);
    }

    protected virtual SemanticExpression RewriteBinaryOperation(SemanticBinaryOperation biop)
    {
        var right = RewriteExpression(biop.Right);
        var left  = RewriteExpression(biop.Left);
        if (right == biop.Right && left == biop.Left)
            return biop;

        return new SemanticBinaryOperation(right, biop.Operator, left, biop.Span);
    }

    protected virtual SemanticExpression RewriteTernaryOperation(SemanticTernaryOperation terop)
    {
        var condition = RewriteExpression(terop.Condition);
        var trueExpr  = RewriteExpression(terop.TrueExpression);
        var falseExpr = RewriteExpression(terop.FalseExpression);
        if (condition == terop.Condition && trueExpr == terop.TrueExpression && falseExpr == terop.FalseExpression)
            return terop;

        return new SemanticTernaryOperation(condition, trueExpr, falseExpr, terop.Span);
    }

    protected virtual SemanticExpression RewriteConversionOperation(SemanticConversionOperation cop)
    {
        var expr = RewriteExpression(cop.Expression);
        if (expr == cop.Expression)
            return cop;

        return new SemanticConversionOperation(expr, cop.Target, cop.ConversionKind, cop.Span);
    }
}
