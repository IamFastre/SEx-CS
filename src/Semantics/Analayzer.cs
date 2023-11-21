using SEx.AST;
using SEx.Diagnose;
using SEx.Parse;
using SEx.Generic;
using SEx.Namespaces;
using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal class Analyzer
{
    public Diagnostics        Diagnostics { get; }
    public Scope              Scope       { get; }
    public Expression         SimpleTree  { get; }
    public SemanticExpression? Tree       { get; protected set; }

    public Analyzer(Expression expr, Diagnostics? diagnostics = null, Scope? scope = null)
    {
        SimpleTree  = expr;
        Diagnostics = diagnostics ?? new();
        Scope       = scope ?? new(Diagnostics);
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.TypeError,
                        ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Analyzer);

    public SemanticExpression Analyze()
    {
        return Tree = BindExpression(SimpleTree);
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

    private SemanticLiteral BindLiteral(Literal literal) => new(literal);

    private SemanticName BindName(Name n)
    {
        return new(n, Scope.ResolveType(n));
    }

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
        return new(aexpr.Assignee, aexpr.Equal, expr);
    }

    private SemanticAssignment BindCompoundAssignExpression(CompoundAssignmentExpression caexpr)
    {
        var expr = BindBinaryExpression(new BinaryExpression(caexpr.Assignee, caexpr.Operator, caexpr.Expression));
        return new(caexpr.Assignee, caexpr.Operator, expr);
    }
}
