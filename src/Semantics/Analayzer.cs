using SEx.AST;
using SEx.Diagnose;
using SEx.Scoping;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Generic.Constants;

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

            default:
                throw new Exception($"Unrecognized statement kind: {stmt.Kind}");
        }
    }

    private SemanticIfStatement BindIfStatement(IfStatement stmt)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(stmt.Condition);

        if (condition.Type != ValType.Boolean)
            Except($"Condition is not of type {CONSTS.BOOLEAN}", condition.Span);

        var thenStmt  = BindStatement(stmt.Then);

        if (stmt.ElseClause is not null)
            elseClause = BindElseClause(stmt.ElseClause);

        return new(stmt.If, condition, thenStmt, elseClause);
    }

    private SemanticElseClause BindElseClause(ElseClause cls)
    {
        var elseStmt = BindStatement(cls.Body);
        return new(cls.Else, elseStmt);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement stmt)
    {
        List<SemanticStatement> statements = new();

        foreach (var statement in stmt.Body)
            statements.Add(BindStatement(statement));

        return new(stmt.OpenBrace, statements.ToArray(), stmt.CloseBrace);
    }

    private SemanticDeclarationStatement BindDeclarationStatement(DeclarationStatement stmt)
    {
        var expr = stmt.Expression is not null ? BindExpression(stmt.Expression) : null;
        return new(stmt, stmt.Type, expr);
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement stmt)
    {
        var expr = BindExpression(stmt.Expression);
        return new(expr);
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

            case NodeKind.Name:
                return BindName((Name) expr);

            case NodeKind.ParenExpression:
                return BindParenExpression((ParenExpression) expr);

            case NodeKind.UnaryOperation:
                return BindUnaryExpression((UnaryExpression) expr);

            case NodeKind.BinaryOperation:
                return BindBinaryExpression((BinaryExpression) expr);

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

    private SemanticName BindName(Name n)
        => new(n, Scope.ResolveType(n));

    private SemanticParenExpression BindParenExpression(ParenExpression pe)
    {
        var expr = pe.Expression is null ? null : BindExpression(pe.Expression);
        return new(pe.OpenParen, expr, pe.CloseParen);
    }

    private SemanticExpression BindUnaryExpression(UnaryExpression uop)
    {
        var operand = BindExpression(uop.Operand);
        var opKind  = SemanticUnaryOperation.GetOperationKind(uop.Operator.Kind, operand.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{uop.Operator.Value}' on type '{operand.Type.str()}'", uop.Span);
            return operand;
        }

        return new SemanticUnaryOperation(uop.Operator, operand, opKind);
    }

    private SemanticExpression BindBinaryExpression(BinaryExpression biop)
    {
        var left   = BindExpression(biop.LHS);
        var right  = BindExpression(biop.RHS);
        var opKind = SemanticBinaryOperation.GetOperationKind(biop.Operator.Kind, left.Type, right.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{biop.Operator.Value}' on types: '{left.Type.str()}' and '{right.Type.str()}'", biop.Span!);
            return new SemanticFailedExpression(new[] { left, right });
        }

        return new SemanticBinaryOperation(left, opKind.Value, right);
    }

    private SemanticAssignment BindAssignExpression(AssignmentExpression aexpr)
    {
        var expr = BindExpression(aexpr.Expression);
        Scope.Types[aexpr.Assignee.Value] = expr.Type;
        return new(aexpr.Assignee, aexpr.Equal, expr);
    }

    private SemanticAssignment BindCompoundAssignExpression(CompoundAssignmentExpression caexpr)
    {
        var expr = BindBinaryExpression(new(caexpr.Assignee, caexpr.Operator, caexpr.Expression));
        return new(caexpr.Assignee, caexpr.Operator, expr);
    }
}
