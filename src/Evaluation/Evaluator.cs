using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Generic.Text;
using SEx.Namespaces;
using SEx.Semantics;

namespace SEx.Evaluate;

internal class Evaluator
{
    public Scope              Scope        { get; }
    public Diagnostics        Diagnostics  { get; }
    public SemanticExpression SemanticTree { get; }
    public LiteralValue       Value        { get; protected set; }


    public Evaluator(Analyzer analyzer)
    {
        Diagnostics  = analyzer.Diagnostics;
        Scope        = analyzer.Scope;
        SemanticTree = analyzer.Tree!;
        Value        = UnknownValue.Template;
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.SyntaxError,
                        ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Evaluator);

    public LiteralValue Evaluate()
        => Value = EvaluateExpression(SemanticTree);

    private LiteralValue EvaluateExpression(SemanticExpression node)
    {
        switch (node.Kind)
            {
                case SemanticKind.Literal:
                    if (node.Type == ValType.Unknown)
                        return UnknownValue.Template;

                    if (node.Type == ValType.Null)
                        return ParseNull((SemanticLiteral) node);

                    if (node.Type == ValType.Boolean)
                        return ParseBool((SemanticLiteral) node);

                    if (node.Type == ValType.Integer)
                        return ParseInt((SemanticLiteral) node);

                    if (node.Type == ValType.Float)
                        return ParseFloat((SemanticLiteral) node);

                    if (node.Type == ValType.Char)
                        return ParseChar((SemanticLiteral) node);

                    if (node.Type == ValType.String)
                        return ParseString((SemanticLiteral) node);

                    break;

                case SemanticKind.Name:
                    return EvaluateName((SemanticName) node);

                case SemanticKind.ParenExpression:
                    return EvaluateParenExpression((SemanticParenExpression) node);

                case SemanticKind.UnaryOperation:
                    return EvaluateUnaryOperation((SemanticUnaryOperation) node);

                case SemanticKind.BinaryOperation:
                    return EvaluateBinaryOperation((SemanticBinaryOperation) node);

                case SemanticKind.AssignExpression:
                    return EvaluateAssignExpression((SemanticAssignment) node);

                case SemanticKind.FailedExpression:
                    return EvaluateFailedExpression((SemanticFailedExpression) node);
        }
        throw new Exception($"Unexpected node type {node?.Kind}");
    }

    private LiteralValue EvaluateParenExpression(SemanticParenExpression expr)
        => expr.Expression is null ? new NullValue() : EvaluateExpression(expr.Expression);

    private LiteralValue EvaluateName(SemanticName name)
        => Scope[name];

    private LiteralValue EvaluateUnaryOperation(SemanticUnaryOperation expr)
    {
        var operand = EvaluateExpression(expr.Operand);

        double _double;

        switch (expr.OperationKind)
        {
            case UnaryOperationKind.Identity:
                return operand;
            
            case UnaryOperationKind.Negation:
                _double = -(double) operand.Value;

                if (operand.Type == ValType.Integer)
                    return new IntegerValue(_double);
                else
                    return new FloatValue(_double);
            
            case UnaryOperationKind.Complement:
                return new BoolValue(!(bool) operand.Value);
        }

        throw new Exception($"Unrecognized unary operation kind: {expr.OperationKind}");
    }

    private LiteralValue EvaluateBinaryOperation(SemanticBinaryOperation expr)
    {
        var left   = EvaluateExpression(expr.Left);
        var kind   = expr.Operator.Kind;
        var right  = EvaluateExpression(expr.Right);

        bool   _bool;
        double _double;
        string _string;

        if (left.Type is ValType.Unknown || right.Type is ValType.Unknown)
            return UnknownValue.Template;

        switch (kind)
        {
            case BinaryOperationKind.NullishCoalescence:
                return left.Value is null ? right : left;

            case BinaryOperationKind.Equality:
            case BinaryOperationKind.Inequality:
                if (left.Value is null && right.Value is null)
                    _bool = kind == BinaryOperationKind.Equality;
                else
                    _bool =
                          kind == BinaryOperationKind.Equality
                        ? left.Value!.Equals(right.Value)

                        : kind == BinaryOperationKind.Inequality
                        ? (!left.Value!.Equals(right.Value))

                        : throw new Exception("This shouldn't occur");

                return new BoolValue(_bool);

            case BinaryOperationKind.AND:
            case BinaryOperationKind.OR:
            case BinaryOperationKind.XOR:
                Int128 _1, _2;
                try
                {
                    (_1, _2) = (Int128.Parse(left.Value.ToString() ?? ""), Int128.Parse(right.Value.ToString() ?? ""));
                }
                catch
                {
                    Except($"Integer too big for bitwise {kind}", expr.Span, ExceptionType.OverflowError);
                    return UnknownValue.Template;
                }

                _double = (double)
                    ( kind == BinaryOperationKind.AND
                    ?  _1 & _2

                    : kind == BinaryOperationKind.OR
                    ?  _1 | _2

                    : kind == BinaryOperationKind.XOR
                    ?  _1 ^  _2

                    : throw new Exception("This shouldn't occur"));

                return new IntegerValue(_double);

            case BinaryOperationKind.LAND:
            case BinaryOperationKind.LOR:
                _bool =
                      kind == BinaryOperationKind.LAND
                    ? (bool) left.Value && (bool) right.Value

                    : kind == BinaryOperationKind.LOR
                    ? (bool) left.Value || (bool) right.Value

                    : throw new Exception("This shouldn't occur");

                return new BoolValue(_bool);

            case BinaryOperationKind.Addition:
            case BinaryOperationKind.Subtraction:
            case BinaryOperationKind.Multiplication:
            case BinaryOperationKind.Division:
            case BinaryOperationKind.Modulo:
            case BinaryOperationKind.Power:
                _double =
                      kind == BinaryOperationKind.Addition
                    ? (double) left.Value + (double) right.Value

                    : kind == BinaryOperationKind.Subtraction
                    ? (double) left.Value - (double) right.Value

                    : kind == BinaryOperationKind.Multiplication
                    ? (double) left.Value * (double) right.Value

                    : kind == BinaryOperationKind.Division
                    ? (double) left.Value / (double) right.Value

                    : kind == BinaryOperationKind.Modulo
                    ? (double) left.Value % (double) right.Value

                    : kind == BinaryOperationKind.Power
                    ? Math.Pow((double) left.Value, (double) right.Value)
            
                    : throw new Exception("This shouldn't occur");

                if (double.IsInteger(_double))
                    return new IntegerValue(_double);
                else
                    return new FloatValue(_double);

            //=====================================================================//

            case BinaryOperationKind.CharAddition:
            case BinaryOperationKind.CharSubtraction:
                var aos = (double) (left.Type == ValType.Integer ? left.Value : right.Value);
                var cr1 = ( char ) (left.Type == ValType.Char    ? left.Value : right.Value);

                _double = kind == BinaryOperationKind.CharAddition
                    ? aos + cr1
                    : aos - cr1;

                return new CharValue((char) (int) Math.Abs(_double));


            //=====================================================================//

            case BinaryOperationKind.StringConcatenation:
                _string = left.Value.ToString() + right.Value.ToString();
                return new StringValue(_string);

            case BinaryOperationKind.StringMultiplication:
                var num = (double) (left.Type == ValType.Integer ? left.Value : right.Value);
                var str = (string) (left.Type == ValType.String  ? left.Value : right.Value);

                _string = string.Empty;

                if (num < 0)
                    str = new(str.Reverse().ToArray());

                for (int i = 0; i < Math.Abs(num); i++)
                    _string += str;

                return new StringValue(_string);

            case BinaryOperationKind.Inclusion:
                _bool = right.Value.ToString()!.Contains(left.Value.ToString()!);
                return new BoolValue(_bool);
        }        

        throw new Exception($"Unrecognized binary operation kind: {expr.Operator}");
    }

    private LiteralValue EvaluateAssignExpression(SemanticAssignment node)
    {
        var val = EvaluateExpression(node.Expression);

        if (val.Type is ValType.Unknown)
        {
            if (!Scope.Contains(node.Assignee))
                return val;
        }
        else
            Scope.Assign(node.Assignee, val);

        return Scope.Resolve(node.Assignee);
    }

    private LiteralValue EvaluateFailedExpression(SemanticFailedExpression fe)
    {
        foreach (var expr in fe.Expressions)
            EvaluateExpression(expr);

        return UnknownValue.Template;
    }

    //=====================================================================//
    //=====================================================================//
    //=====================================================================//

    private LiteralValue ParseNull(SemanticLiteral literal)
    {
        if (literal.Value != CONSTS.NULL)
            Except($"Error ocurred while parsing null", literal.Span!, ExceptionType.InternalError);

        return new NullValue();
    }

    private LiteralValue ParseBool(SemanticLiteral literal)
    {
        if (literal.Value == CONSTS.TRUE)
            return new BoolValue(true);

        if (literal.Value == CONSTS.FALSE)
            return new BoolValue(false);

        Except($"Error ocurred while parsing boolean", literal.Span!, ExceptionType.InternalError);
        return UnknownValue.Template;
    }

    private LiteralValue ParseInt(SemanticLiteral literal)
    {
        double num;
        try
        {
            num = double.Parse(literal.Value);
            return new IntegerValue(num);
        }
        catch
        {
            Except($"Error ocurred while parsing integer", literal.Span!, ExceptionType.InternalError);
        }

        return UnknownValue.Template;
    }

    private LiteralValue ParseFloat(SemanticLiteral literal)
    {
        double num;
        try
        {
            num = double.Parse("fF".Contains(literal.Value[^1]) ? literal.Value[0..^1] : literal.Value);
            return new FloatValue(num);
        }
        catch
        {
            Except($"Error ocurred while parsing float", literal.Span!, ExceptionType.InternalError);
        }

        return UnknownValue.Template;
    }

    private LiteralValue ParseChar(SemanticLiteral literal)
    {
        string? str = Prepare(literal);

        if (str is null)
            return UnknownValue.Template;

        else if (!char.TryParse(str, out char chr))
            Except($"Invalid character string", literal.Span!);

        else
            return new CharValue(chr);

        return UnknownValue.Template;
    }

    private LiteralValue ParseString(SemanticLiteral literal)
    {
        string? str = Prepare(literal);
        if (str is not null)
            return new StringValue(str);

        return UnknownValue.Template;
    }


    string? Prepare(SemanticLiteral node)
    {
        try { return node.Value.Unescape()[1..^1]; }
        catch { Except($"Invalid escape sequence", node.Span!, ExceptionType.StringParseError); }

        return null;
    }
}