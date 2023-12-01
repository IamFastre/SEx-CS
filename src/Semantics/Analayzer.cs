using SEx.AST;
using SEx.Diagnose;
using SEx.Scoping;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Parse;

namespace SEx.Semantics;

internal class Analyzer
{
    public Diagnostics                Diagnostics { get; }
    public Scope                      Scope       { get; }
    public ProgramStatement           SimpleTree  { get; }
    public SemanticProgramStatement?  Tree        { get; protected set; }

    public Analyzer(ProgramStatement stmt, Diagnostics? diagnostics = null, Scope? scope = null)
    {
        SimpleTree  = stmt;
        Diagnostics = diagnostics ?? new();
        Scope       = scope ?? new(Diagnostics);
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.TypeError,
                        ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Analyzer);

    public SemanticStatement Analyze()
    {
        return Tree = BindProgram(SimpleTree);
    }

    private SemanticProgramStatement BindProgram(ProgramStatement stmt)
    {
        List<SemanticStatement> statements = new();

        foreach (var statement in stmt.Body)
            statements.Add(BindStatement(statement));

        return new(statements.ToArray());
    }

    private SemanticStatement BindStatement(Statement stmt)
    {
        switch (stmt.Kind)
        {
            case NodeKind.ExpressionStatement:
                return BindExpressionStatement((ExpressionStatement) stmt);

            case NodeKind.DeclarationStatement:
                return BindDeclarationStatement((DeclarationStatement) stmt);

            case NodeKind.BlockStatement:
                return BindBlockStatement((BlockStatement) stmt);

            case NodeKind.IfStatement:
                return BindIfStatement((IfStatement) stmt);

            case NodeKind.WhileStatement:
                return BindWhileStatement((WhileStatement) stmt);

            case NodeKind.ForStatement:
                return BindForStatement((ForStatement) stmt);

            default:
                throw new Exception($"Unrecognized statement kind: {stmt.Kind}");
        }
    }

    private SemanticIfStatement BindIfStatement(IfStatement @is)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(@is.Condition, ValType.Boolean | ValType.Unknown);
        var thenStmt  = BindStatement(@is.Then);

        if (@is.ElseClause is not null)
            elseClause = BindElseClause(@is.ElseClause);

        return new(@is.If, condition, thenStmt, elseClause);
    }

    private SemanticWhileStatement BindWhileStatement(WhileStatement ws)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(ws.Condition, ValType.Boolean | ValType.Unknown);
        var thenStmt  = BindStatement(ws.Body);

        if (ws.ElseClause is not null)
            elseClause = BindElseClause(ws.ElseClause);

        return new(ws.While, condition, thenStmt, elseClause);
    }

    private SemanticElseClause BindElseClause(ElseClause ec)
    {
        var elseStmt = BindStatement(ec.Body);
        return new(ec.Else, elseStmt);
    }

    private SemanticForStatement BindForStatement(ForStatement fs)
    {
        var variable = BindName(fs.Variable);
        var iterable = BindExpression(fs.Iterable);
        var body     = BindStatement(fs.Body);

        return new(fs.For, variable, iterable, body);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        List<SemanticStatement> statements = new();

        foreach (var statement in bs.Body)
            statements.Add(BindStatement(statement));

        return new(bs.OpenBrace, statements.ToArray(), bs.CloseBrace);
    }

    private SemanticDeclarationStatement BindDeclarationStatement(DeclarationStatement ds)
    {
        var expr = ds.Expression is not null ? BindExpression(ds.Expression) : null;
        Scope.Types[ds.Name.Value] = expr?.Type
                                   ?? SemanticDeclarationStatement.GetNameType(ds.Type?.Value);

        return new(ds, ds.Type, expr);
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement es)
    {
        var expr = BindExpression(es.Expression);
        return new(expr);
    }

    private SemanticExpression BindExpression(Expression expr, ValType expected)
    {
        var val = BindExpression(expr);
        if (!expected.HasFlag(val.Type))
        {
            if (expected.HasFlag(ValType.Unknown) && expected != ValType.Unknown)
                expected -= ValType.Unknown;
            Except($"Expected an expression of type '{expected.str()}'", expr.Span);
        }
        
        return val;
    }

    private SemanticExpression BindExpression(Expression expr)
    {
        switch (expr.Kind)
        {
            case NodeKind.Unknown:
            case NodeKind.Null:
            case NodeKind.Boolean:
            case NodeKind.Integer:
            case NodeKind.Float:
            case NodeKind.Char:
            case NodeKind.String:
                return BindLiteral((Literal) expr);

            case NodeKind.Range:
                return BindRange((RangeLiteral) expr);

            case NodeKind.Name:
                return BindName((NameLiteral) expr);

            case NodeKind.List:
                return BindList((ListLiteral) expr);

            case NodeKind.ParenthesizedExpression:
                return BindParenExpression((ParenthesizedExpression) expr);

            case NodeKind.IndexingExpression:
                return BindIndexingExpression((IndexingExpression) expr);

            case NodeKind.UnaryOperation:
                return BindUnaryOperation((UnaryOperation) expr);

            case NodeKind.CountingOperation:
                return BindCountingOperation((CountingOperation) expr);

            case NodeKind.BinaryOperation:
                return BindBinaryOperation((BinaryOperation) expr);

            case NodeKind.TernaryOperation:
                return BindTernaryOperation((TernaryOperation) expr);

            case NodeKind.AssignmentExpression:
                return BindAssignExpression((AssignmentExpression) expr);

            case NodeKind.CompoundAssignmentExpression:
                return BindCompoundAssignExpression((CompoundAssignmentExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticLiteral BindLiteral(Literal literal)
        => new(literal);

    private SemanticRange BindRange(RangeLiteral r)
    {
        var start = BindExpression(r.Start, ValType.Number);
        var end   = BindExpression(r.End, ValType.Number);
        var step  = r.Step is null ? null : BindExpression(r.Step, ValType.Number);

        return new(start, end, step);
    }

    private SemanticName BindName(NameLiteral n)
        => new(n, Scope.ResolveType(n.Value));

    private SemanticList BindList(ListLiteral ll)
    {
        if (ll.Elements.Length > 0)
        {
            List<SemanticExpression> expressions = new();
            var arRef = BindExpression(ll.Elements.First());
            expressions.Add(arRef);

            foreach (var elem in ll.Elements[1..])
            {
                var expr = BindExpression(elem);

                if (expr.Type == arRef.Type)
                    expressions.Add(expr);
                else
                    Except($"list of type '{arRef.Type.str()}' can't have a '{expr.Type.str()}' element", ll.Span);
            }

            return new(expressions.ToArray(), arRef.Type, ll.Span);
        }

        return new(Array.Empty<SemanticExpression>(), ValType.Any, ll.Span);
    }

    private SemanticParenExpression BindParenExpression(ParenthesizedExpression pe)
    {
        var expr = pe.Expression is null ? null : BindExpression(pe.Expression);
        return new(pe.OpenParen, expr, pe.CloseParen);
    }

    private SemanticExpression BindIndexingExpression(IndexingExpression ie)
    {
        var iterable    = BindExpression(ie.Iterable);
        var index       = BindExpression(ie.Index);
        var elementType = SemanticIndexingExpression.GetElementType(iterable.Type, index.Type);

        if (elementType is null)
        {
            Except($"Can't perform indexing on '{iterable.Type.str()}'", ie.Iterable.Span);
            return new SemanticFailedExpression(new[] { iterable, index });
        }

        return new SemanticIndexingExpression(iterable, index, elementType ?? ValType.Unknown, ie.Span);
    }

    private SemanticExpression BindUnaryOperation(UnaryOperation uop)
    {
        var operand = BindExpression(uop.Operand);
        var opKind  = SemanticUnaryOperation.GetOperationKind(uop.Operator.Kind, operand.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{uop.Operator.Value}' on type '{operand.Type.str()}'", uop.Span);
            return new SemanticFailedExpression(new[] { operand });
        }

        return new SemanticUnaryOperation(operand, opKind.Value, uop.Span);
    }

    private SemanticExpression BindCountingOperation(CountingOperation co)
    {
        var name   = BindName(co.Name);
        var opKind = SemanticCountingOperation.GetOperationKind(co.Operator.Kind, name.Type, co.ReturnAfter);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{co.Operator.Value}' on type '{name.Type.str()}'", co.Span);
            return new SemanticFailedExpression(new[] { name });
        }

        return new SemanticCountingOperation(name, opKind.Value, co.Span);
    }

    private SemanticExpression BindBinaryOperation(BinaryOperation biop)
    {
        var left   = BindExpression(biop.LHS);
        var right  = BindExpression(biop.RHS);
        var opKind = SemanticBinaryOperation.GetOperationKind(biop.Operator.Kind, left.Type, right.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{biop.Operator.Value}' on types: '{left.Type.str()}' and '{right.Type.str()}'", biop.Span);
            return new SemanticFailedExpression(new[] { left, right });
        }

        return new SemanticBinaryOperation(left, opKind.Value, right);
    }

    private SemanticTernaryOperation BindTernaryOperation(TernaryOperation terop)
    {
        var condition = BindExpression(terop.Condition, ValType.Boolean);
        var trueExpr  = BindExpression(terop.TrueExpression);
        var falseExpr = BindExpression(terop.FalseExpression);

        if (trueExpr.Type != falseExpr.Type)
            Except($"Types '{trueExpr.Type.str()}' and '{falseExpr.Type.str()}' don't match in ternary operation", terop.Span);

        return new SemanticTernaryOperation(condition, trueExpr, falseExpr);
    }

    private SemanticAssignment BindAssignExpression(AssignmentExpression aexpr)
    {
        var expr = BindExpression(aexpr.Expression);
        var name = BindName(aexpr.Assignee);
        Scope.Types[aexpr.Assignee.Value] = expr.Type;
        return new(name, aexpr.Equal, expr);
    }

    private SemanticAssignment BindCompoundAssignExpression(CompoundAssignmentExpression caexpr)
    {
        var expr = BindBinaryOperation(new(caexpr.Assignee, caexpr.Operator, caexpr.Expression));
        var name = BindName(caexpr.Assignee);

        return new(name, caexpr.Operator, expr);
    }
}
