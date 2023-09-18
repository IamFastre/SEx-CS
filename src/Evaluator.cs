using System.Text.RegularExpressions;
using SEx.AST;
using SEx.Analysis;
using SEx.Parse;
using SEx.Generic;

namespace SEx.Evaluate;

public class NodeValue
{
    public NodeValue(dynamic value, Node node) 
    {
        Value = value;
        Node = node;
    }

    public dynamic Value { get; }
    public Node Node { get; }
    public NodeType Type => Value switch
    {
        null    => NodeType.Unknown,
        Int128  => NodeType.Integer,
        decimal => NodeType.Float,
        char    => NodeType.Char,
        string  => NodeType.String,

        _ => throw new Exception("Unknown value")
    };

    public override string ToString()
    {
        return $"{Type} >> {Value}";
    }

    public static readonly NodeValue Unknown = new(null!, Literal.Unknown);
}

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

    public void Except(string message, Span span, ExceptionType type = ExceptionType.SyntaxError)
    {
        Diagnostics.Add(
            type,
            message,
            span
        );
    }

    public NodeValue Evaluate(Node tree)
    {
        return EvaluateStatement((Statement) tree);
    }

    public NodeValue EvaluateStatement(Statement statement)
    {
        return EvaluateExpression((Expression) statement);
    }

    public NodeValue EvaluateExpression(Expression node)
    {
        dynamic? value;
        switch (node)
            {
                case IntLiteral IL:
                    if (!Int128.TryParse(IL.Value, out _))
                    {
                        Except($"Integer is too big", IL.Span!, ExceptionType.OverflowError);
                        return NodeValue.Unknown;
                    }
                    value = Int128.Parse(IL.Value!);
                    return new NodeValue(value, IL);

                case FloatLiteral FL:
                    if (!decimal.TryParse(FL.Value, out _))
                    {
                        Except($"Float is either too big or too precise", FL.Span!, ExceptionType.OverflowError);
                        return NodeValue.Unknown;
                    }
                    value = decimal.Parse(FL.Value!);
                    return new NodeValue(value, FL);

                case CharLiteral CL:
                    string? c = Prepare(CL.Value![1..^1], node);

                    if (c is null) return NodeValue.Unknown;
                    if (!char.TryParse(c, out _))
                    {
                        Except($"Invalid character string", CL.Span!, ExceptionType.TypeError);
                        return NodeValue.Unknown;
                    }
                    value = char.Parse(c);
                    return new NodeValue(value, CL);

                case StringLiteral SL:
                    value = Prepare(SL.Value![1..^1], node);
                    if (value is null) return NodeValue.Unknown;
                    return new NodeValue(value, SL);

                case BinaryExpression be:
                    return EvaluateBinaryOperation(be);

                case ParenExpression pe:
                    return EvaluateParenExpression(pe);

                default:
                    throw new Exception("Unexpected node type");
        }

        string? Prepare(string str, Node node)
        {
            try
            {
                str = Regex.Unescape(str);
            }
            catch
            {
                Except($"Invalid escape sequence", node.Span!, ExceptionType.StringParseError);
                return null;
            }
            return str;
        }
    }

    public NodeValue EvaluateParenExpression(ParenExpression expr)
    {
        dynamic? value = expr.Expression is not null
            ? EvaluateExpression(expr.Expression).Value
            : null;

        return new NodeValue(value, expr);
    }

    public NodeValue EvaluateBinaryOperation(BinaryExpression expr)
    {
        var left  = EvaluateExpression(expr.LHS);
        var op    = expr.Operator.Value;
        var right = EvaluateExpression(expr.RHS);

        // if (left.Value is null || right.Value is null)
        //     return NodeValue.Null;

        dynamic? value;

        if (Check(Mode.ALL, (left.Type, right.Type), NodeType.Integer, NodeType.Float))
        {
            var (operand1, operand2) = (decimal.Parse(left.Value.ToString()), decimal.Parse(right.Value.ToString()));
            switch (op)
            {
                case "+":
                    value = operand1 + operand2;
                    return new NodeValue(Proper(value), expr);
                case "-":
                    value = operand1 - operand2;
                    return new NodeValue(Proper(value), expr);
                case "*":
                    value = operand1 * operand2;
                    return new NodeValue(Proper(value), expr);
                case "/":
                    value = operand1 / operand2;
                    return new NodeValue(Proper(value), expr);
                case "%":
                    value = operand1 % operand2;
                    return new NodeValue(Proper(value), expr);
                case "**":
                    value = Math.Pow((double) operand1, (double) operand2);
                    return new NodeValue(Proper(value), expr);
                default:
                    break;
            }

            static dynamic Proper(dynamic value)
                => Math.Floor(value) == Math.Ceiling(value)
                    ? (Int128 ) value
                    : (decimal) value;
        }

        if (Check(Mode.ANY, (left.Type, right.Type), NodeType.String))
        {
            switch (op)
            {
                case "+":
                    var (operand1, operand2) = (left.Value.ToString(), right.Value.ToString());
                    value = operand1 + operand2;

                    return new NodeValue(value, expr);

                case "*":
                    value   = string.Empty;
                    var str = left.Value is string ? left.Value : right.Value;
                    var num = left.Value is int ? left.Value : right.Value;

                    int _;
                    if (int.TryParse(num, out _) || str is not string) break;

                    for (int i = 0; i < num; i++)
                    {
                        value += str;
                    }
                    return new NodeValue(value, expr);

                default:
                    break;
            }

        }

        if (Check(Mode.ANY, (left.Type, right.Type), NodeType.Char))
        {
            switch (op)
            {
                case "+":
                    if (left.Type is NodeType.Integer || right.Type is NodeType.Integer)
                        value = (char) (left.Value + right.Value);
                    else
                        break;
                    return new NodeValue(value, expr);

                default:
                    break;
            }
        }

        Except($"Cannot perform operation \"{op}\" on {left.Type} and {right.Type}", expr.Span!, ExceptionType.TypeError);
        return NodeValue.Unknown;
    }

    //=====================================================================//
    //=====================================================================//

    public static bool Check(Mode mode, (NodeType, NodeType) nodes, params NodeType[] types)
    {
        NodeType[] operands = {nodes.Item1, nodes.Item2};

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