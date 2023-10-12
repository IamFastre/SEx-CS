using SEx.AST;
using SEx.Diagnose;
using SEx.Parse;
using SEx.Evaluator.Values;
using SEx.Generic;

namespace SEx.Semantics;

internal class Analyzer
{
    public Diagnostics Diagnostics { get; }
    public SyntaxTree Tree { get; }

    public Analyzer(Parser parser) : this(parser.Tree) {}
    public Analyzer(SyntaxTree tree)
    {
        Diagnostics = tree.Diagnostics;
        Tree = tree;
    }

    private void Except(string message, Span span, ExceptionType type = ExceptionType.TypeError)
    {
        Diagnostics.Add(
            type,
            message,
            span
        );
    }

    public SemanticExpression Analyze(Expression expr)
    {
        switch (expr.Kind)
        {
            case NodeKind.Null:
            case NodeKind.Boolean:
            case NodeKind.Integer:
            case NodeKind.Float:
            case NodeKind.Char:
            case NodeKind.String:
                return BindLiteral((Literal) expr);

            case NodeKind.ParenExpression:
                return BindParenExpression((ParenExpression) expr);

            case NodeKind.BinaryOperation:
                return BindBinaryExpression((BinaryExpression) expr);

            case NodeKind.UnaryOperation:
                return BindUnaryExpression((UnaryExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticExpression BindLiteral(Literal literal)
    {
        return new SemanticLiteral(literal);
    }

    private SemanticExpression BindParenExpression(ParenExpression expr)
    {
        throw new NotImplementedException();
    }

    private SemanticExpression BindUnaryExpression(UnaryExpression uop)
    {
        var operand = Analyze(uop.Operand);
        var opKind  = SemanticUnaryOperation.GetOperationKind(uop.Operator.Kind, operand.Type);

        if (opKind is null)
        {
            Except($"Cannot perform unary operation '{uop.Operator.Value}' on type {operand.Type}", uop.Span!);
            return operand;
        }

        return new SemanticUnaryOperation(opKind.Value, operand);
    }

    private SemanticExpression BindBinaryExpression(BinaryExpression biop)
    {
        var left   = Analyze(biop.LHS);
        var right  = Analyze(biop.RHS);
        var opKind = SemanticBinaryOperation.GetOperationKind(biop.Operator.Kind, left.Type, right.Type);

        if (opKind is null)
        {
            Except($"Cannot perform unary operation '{biop.Operator.Value}' on types: {left.Type} and {right.Type}", biop.Span!);
            return left;
        }

        return new SemanticBinaryOperation(left, opKind.Value, right);
    }
}

public enum SemanticKind
{
    Literal,
    UnaryOperation,
    BinaryOperation,
}

internal abstract class SemanticNode
{
    public abstract SemanticKind Kind { get; }

    public static ValType ToValueKind(NodeKind kind) => kind switch
    {
        NodeKind.Null    => ValType.Null,
        NodeKind.Boolean => ValType.Boolean,
        NodeKind.Integer => ValType.Integer,
        NodeKind.Float   => ValType.Float,
        NodeKind.Char    => ValType.Char,
        NodeKind.String  => ValType.String,

        _ => throw new Exception("Unknown literal kind"),
    };
}

internal abstract class SemanticExpression : SemanticNode
{
    public abstract ValType Type { get; }
}

internal sealed class SemanticLiteral : SemanticExpression
{
    public override SemanticKind Kind => SemanticKind.Literal;
    public object Value { get; }
    public override ValType Type { get; }

    public SemanticLiteral(Literal literal)
    {
        Value = literal.Value;
        Type  = ToValueKind(literal.Kind);
    }
}