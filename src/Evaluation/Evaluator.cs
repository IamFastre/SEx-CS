using SEx.AST;
using SEx.Diagnose;
using SEx.Parse;
using SEx.Generic;
using SEx.Lex;
using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Evaluation;

public class Evaluator
{
    public SyntaxTree Tree { get; }
    public NodeValue Value { get; }
    public Diagnostics Diagnostics;

    public Evaluator(string source, Diagnostics diagnostics)
    {
        Diagnostics = diagnostics ?? new Diagnostics();
        var parser = new Parser(source, Diagnostics);
        Tree = parser.Tree;
        Value = Evaluate(Tree.Root!) ?? NodeValue.Unknown;
    }

    public Evaluator(Parser parser)
    {
        Diagnostics = parser.Diagnostics;
        Tree = parser.Tree;
        Value = Evaluate(Tree.Root!) ?? NodeValue.Unknown;
    }

    public void Except(string message, Span span, ExceptionType type = ExceptionType.SyntaxError)
    {
        Diagnostics.Add(
            type,
            message,
            span
        );
    }

    private NodeValue Evaluate(Node tree)
    {
        return EvaluateStatement((Statement) tree);
    }

    private NodeValue EvaluateStatement(Statement statement)
    {
        return EvaluateExpression((Expression) statement);
    }

    private NodeValue EvaluateExpression(Expression node)
    {
        dynamic? value;
        switch (node)
            {
                case BooleanLiteral BL:
                    value = BL.Value == CONSTS.TRUE;
                    return new NodeValue(value, BL);

                case IntLiteral IL:
                    if (!Int128.TryParse(IL.Value, out _))
                    {
                        Except($"Integer is too big", IL.Span!, ExceptionType.OverflowError);
                        return NodeValue.Unknown;
                    }
                    value = Int128.Parse(IL.Value!);
                    return new NodeValue(value, IL);

                case FloatLiteral FL:
                    if (!double.TryParse(FL.Value, out _))
                    {
                        Except($"Float is either too big or too precise", FL.Span!, ExceptionType.OverflowError);
                        return NodeValue.Unknown;
                    }
                    value = double.Parse(FL.Value!);
                    return new NodeValue(value, FL);

                case CharLiteral CL:
                    string? c = Prepare(CL.Value!, node);

                    if (c is null) return NodeValue.Unknown;
                    if (!char.TryParse(c, out _))
                    {
                        Except($"Invalid character string", CL.Span!, ExceptionType.TypeError);
                        return NodeValue.Unknown;
                    }
                    value = char.Parse(c);
                    return new NodeValue(value, CL);

                case StringLiteral SL:
                    value = Prepare(SL.Value!, node);
                    if (value is null) return NodeValue.Unknown;
                    return new NodeValue(value, SL);

                case UnaryExpression ue:
                    return EvaluateUnaryOperation(ue);

                case BinaryExpression be:
                    return EvaluateBinaryOperation(be);

                case ParenExpression pe:
                    return EvaluateParenExpression(pe);

                case IdentifierLiteral IL:
                    return NodeValue.Unknown;

                case null:
                    return NodeValue.Unknown;

                default:
                    throw new Exception($"Unexpected node type {node?.Kind}");
        }

        string? Prepare(string str, Node node)
        {
            try { return str.ToEscaped(); }
            catch
            {
                Except($"Invalid escape sequence", node.Span!, ExceptionType.StringParseError);
                return null;
            }
        }
    }

    private NodeValue EvaluateParenExpression(ParenExpression expr)
    {
        dynamic? value = expr.Expression is not null
            ? EvaluateExpression(expr.Expression).Value
            : null;

        return new NodeValue(value, expr);
    }

    private NodeValue EvaluateUnaryOperation(UnaryExpression unExpr)
    {
        var op      = unExpr.Operator;
        var operand = EvaluateExpression(unExpr.Operand);

        dynamic? value;

        if (CheckKind(operand.Kind, NodeKind.Integer, NodeKind.Float))
        {
            switch (op.Kind)
            {
                case TokenKind.Plus:
                    value = operand.Value;
                    return new NodeValue(value, unExpr);

                case TokenKind.Minus:
                    value = -operand.Value;
                    return new NodeValue(value, unExpr);
            }
        }

        Except($"Cannot perform operation \"{op.Value}\" on {operand.Kind}", unExpr.Span!, ExceptionType.TypeError);
        return NodeValue.Unknown;
    }

    private NodeValue EvaluateBinaryOperation(BinaryExpression binExpr)
    {
        var left  = EvaluateExpression(binExpr.LHS);
        var op    = binExpr.Operator;
        var right = EvaluateExpression(binExpr.RHS);

        if (op.Kind.IsBoolOp())
            return EvaluateConditionalOperation(binExpr);

        dynamic? value;

        if (CheckKinds(Mode.ALL, (left.Kind, right.Kind), NodeKind.Integer, NodeKind.Float))
        {
            double operand1, operand2;
            try { (operand1, operand2) = (double.Parse(left.ToString()), double.Parse(right.ToString())); }
            catch
            {
                Except($"Number is either too big or too precise", binExpr.Span!, ExceptionType.OverflowError);
                return NodeValue.Unknown;
            }

            switch (op.Kind)
            {
                case TokenKind.Plus:
                    value = operand1 + operand2;
                    return new NodeValue(Proper(value), binExpr);

                case TokenKind.Minus:
                    value = operand1 - operand2;
                    return new NodeValue(Proper(value), binExpr);

                case TokenKind.Asterisk:
                    value = operand1 * operand2;
                    return new NodeValue(Proper(value), binExpr);

                case TokenKind.ForwardSlash:
                    value = operand1 / operand2;
                    return new NodeValue(Proper(value), binExpr);

                case TokenKind.Percent:
                    value = operand1 % operand2;
                    return new NodeValue(Proper(value), binExpr);

                case TokenKind.Power:
                    value = Math.Pow((double) operand1, (double) operand2);
                    return new NodeValue(Proper(value), binExpr);
            }
        }

        if (CheckKinds(Mode.ALL, (left.Kind, right.Kind), NodeKind.Integer, NodeKind.Boolean))
        {
            dynamic operand1, operand2;
            if (left.Kind == NodeKind.Boolean && right.Kind == NodeKind.Boolean)
                (operand1, operand2) = (left.Value, right.Value);
            else
                (operand1, operand2) = (GetInt(left), GetInt(right));

            switch (op.Kind)
            {
                case TokenKind.AND:
                    value = operand1 & operand2;
                    return new NodeValue(Proper(value, true), binExpr);

                case TokenKind.OR:
                    value = operand1 | operand2;
                    return new NodeValue(Proper(value, true), binExpr);

                case TokenKind.XOR:
                    value = operand1 ^ operand2;
                    return new NodeValue(Proper(value, true), binExpr);
            }
        }

        if (CheckKinds(Mode.ANY, (left.Kind, right.Kind), NodeKind.String))
        {
            switch (op.Kind)
            {
                case TokenKind.Plus:
                    var (operand1, operand2) = (left.ToString(), right.ToString());
                    value = operand1 + operand2;

                    return new NodeValue(value, binExpr);

                case TokenKind.Asterisk:
                    value   = string.Empty;
                    string str;
                    int num;
                    try
                    {
                        str = left.Kind is NodeKind.String
                            ? (string) left.Value
                            : (string) right.Value;

                        num = left.Kind is NodeKind.Integer
                            ? (int) left.Value
                            : right.Kind is NodeKind.Integer
                            ? (int) right.Value
                            : throw new Exception("Neither is integer");
                    }
                    catch { break; }

                    if (num < 0)
                    {
                        num = -num;
                        char[] charArray = str.ToCharArray();
                        Array.Reverse(charArray);
                        str = new string(charArray);
                    }

                    for (int i = 0; i < num; i++)
                    {
                        value += str;
                    }
                    return new NodeValue(value, binExpr);
            }

        }

        if (CheckKinds(Mode.ANY, (left.Kind, right.Kind), NodeKind.Char))
        {
            switch (op.Kind)
            {
                case TokenKind.Plus:
                    if (left.Kind is NodeKind.Integer || right.Kind is NodeKind.Integer)
                        value = (char) (left.Value + right.Value);
                    else
                        break;
                    return new NodeValue(value, binExpr);

                case TokenKind.Minus:
                    if (left.Kind is NodeKind.Integer || right.Kind is NodeKind.Integer)
                        value = (char) (left.Value - right.Value);
                    else
                        break;
                    return new NodeValue(value, binExpr);
            }
        }

        if (CheckKinds(Mode.ANY, (left.Kind, right.Kind), NodeKind.Boolean))
        {
            switch (op.Kind)
            {
                case TokenKind.Plus:
                    value = GetInt(left) + GetInt(right);
                    return new NodeValue(value, binExpr);
            }
        }

        Except($"Cannot perform operation \"{op.Value}\" on {left.Kind} and {right.Kind}", binExpr.Span!, ExceptionType.TypeError);
        return NodeValue.Unknown;
    }

    private NodeValue EvaluateConditionalOperation(BinaryExpression binExpr)
    {
        var left  = GetBool(EvaluateExpression(binExpr.LHS));
        var op    = binExpr.Operator;
        var right = GetBool(EvaluateExpression(binExpr.RHS));

        bool? value;
        switch (op.Kind)
        {
            case TokenKind.LogicalAND:
                value = left && right;
                break;

            case TokenKind.LogicalOR:
                value = left || right;
                break;
            
            case TokenKind.LogicalXOR:
                value = (left || right) && !(left && right);
                break;

            case TokenKind.AND:
                value = right & left;
                return new NodeValue(value, binExpr);

            case TokenKind.OR:
                value = right | left;
                return new NodeValue(value, binExpr);

            case TokenKind.XOR:
                value = right ^ left;
                return new NodeValue(value, binExpr);

            default:
                throw new Exception($"Operator {op.Kind} is not a boolean operator");
        }
        return new NodeValue(value, binExpr);
    }

    //=====================================================================//
    //=====================================================================//

    public static bool GetBool(NodeValue value)
        => value.Kind switch
        {
            NodeKind.Unknown => false,
            NodeKind.Boolean => (bool) value.Value,
            NodeKind.Integer => (bool) (value.Value != 0),
            NodeKind.Char    => (bool) (value.Value != '\0'),
            NodeKind.String  => (bool) (value.Value != ""),
            _ => true,
        };

    public static Int128 GetInt(NodeValue value)
    {
        return value.Kind is NodeKind.Integer
            ? value.Value
            : value.Kind is NodeKind.Boolean
            ? (value.Value ? 1 : 0)
            : throw new Exception($"Can't convert {value.Kind} to int");
    }

    public static dynamic Proper(dynamic value, bool canBeBool = false)
    {
        if (canBeBool && value is bool)
            return value;

        return Math.Floor((double) value) == Math.Ceiling((double) value)
                ? (Int128)value
                : (double)value;
    }

    public static bool CheckKind(NodeKind node, params NodeKind[] types)
        => types.Contains(node);

    public static bool CheckKinds(Mode mode, (NodeKind, NodeKind) nodes, params NodeKind[] types)
    {
        NodeKind[] operands = {nodes.Item1, nodes.Item2};

        if (mode is Mode.ANY)
            return operands.Any(v => types.Contains(v));

        if (mode is Mode.ALL)
            return operands.All(v => types.Contains(v));

        throw new ArgumentException($"Unknown check mode: {mode}");
    }

    public enum Mode
    {
        ANY,
        ALL
    }
}