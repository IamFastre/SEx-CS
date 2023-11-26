using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Generic.Text;
using SEx.Scoping;
using SEx.Semantics;

namespace SEx.Evaluate;

internal class Evaluator
{
    public Scope                    Scope        { get; }
    public Diagnostics              Diagnostics  { get; }
    public SemanticProgramStatement SemanticTree { get; }
    public LiteralValue             Value        { get; protected set; }

    public Evaluator(Analyzer analyzer)
    {
        Scope        = analyzer.Scope;
        Diagnostics  = analyzer.Diagnostics;
        SemanticTree = analyzer.Tree!;
        Value        = UnknownValue.Template;
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.SyntaxError,
                        ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Evaluator);

    public LiteralValue Evaluate()
        => Value = EvaluateProgram(SemanticTree);

    private LiteralValue EvaluateProgram(SemanticProgramStatement stmt)
    {
        LiteralValue lastValue = VoidValue.Template;
        foreach (var statement in stmt.Body)
            lastValue = EvaluateStatement(statement);

        return lastValue;
    }

    private LiteralValue EvaluateStatement(SemanticStatement stmt)
    {
        switch (stmt.Kind)
            {
                case SemanticKind.ExpressionStatement:
                    return EvaluateExpressionStatement((SemanticExpressionStatement) stmt);

                case SemanticKind.DeclarationStatement:
                    return EvaluateDeclarationStatement((SemanticDeclarationStatement) stmt);

                case SemanticKind.BlockStatement:
                    return EvaluateBlockStatement((SemanticBlockStatement) stmt);

                case SemanticKind.IfStatement:
                    return EvaluateIfStatement((SemanticIfStatement) stmt);
        }

        throw new Exception($"Unexpected statement type {stmt?.Kind}");
    }

    private LiteralValue EvaluateIfStatement(SemanticIfStatement stmt)
    {
        LiteralValue value = VoidValue.Template;
        var conditionVal = EvaluateExpression(stmt.Condition);

        if ((bool) conditionVal.Value)
            value = EvaluateStatement(stmt.Then);
        else
            if (stmt.ElseClause is not null)
                value = EvaluateStatement(stmt.ElseClause.Body);

        return value;

    }

    private LiteralValue EvaluateBlockStatement(SemanticBlockStatement stmt)
    {
        foreach (var statement in stmt.Body)
            EvaluateStatement(statement);

        return VoidValue.Template;
    }

    private LiteralValue EvaluateDeclarationStatement(SemanticDeclarationStatement stmt)
    {
        var value = stmt.Expression is null ? UndefinedValue.New(stmt.NameType) : EvaluateExpression(stmt.Expression);
        if (stmt.Name.Value.Length > 0)
            Scope.Declare(stmt, value);
        return VoidValue.Template;
    }

    private LiteralValue EvaluateExpressionStatement(SemanticExpressionStatement stmt)
        => EvaluateExpression(stmt.Expression);

    private LiteralValue EvaluateExpression(SemanticExpression? expr)
    {
        if (expr is null)
            return UnknownValue.Template;

        switch (expr.Kind)
            {
                case SemanticKind.Literal:
                    if (expr.Type == ValType.Unknown)
                        return UnknownValue.Template;

                    if (expr.Type == ValType.Null)
                        return ParseNull((SemanticLiteral) expr);

                    if (expr.Type == ValType.Boolean)
                        return ParseBool((SemanticLiteral) expr);

                    if (expr.Type == ValType.Integer)
                        return ParseInt((SemanticLiteral) expr);

                    if (expr.Type == ValType.Float)
                        return ParseFloat((SemanticLiteral) expr);

                    if (expr.Type == ValType.Char)
                        return ParseChar((SemanticLiteral) expr);

                    if (expr.Type == ValType.String)
                        return ParseString((SemanticLiteral) expr);

                    break;

                case SemanticKind.Name:
                    return EvaluateName((SemanticName) expr);

                case SemanticKind.ParenExpression:
                    return EvaluateParenExpression((SemanticParenExpression) expr);

                case SemanticKind.UnaryOperation:
                    return EvaluateUnaryOperation((SemanticUnaryOperation) expr);

                case SemanticKind.BinaryOperation:
                    return EvaluateBinaryOperation((SemanticBinaryOperation) expr);

                case SemanticKind.TernaryOperation:
                    return EvaluateTernaryOperation((SemanticTernaryOperation) expr);

                case SemanticKind.AssignExpression:
                    return EvaluateAssignExpression((SemanticAssignment) expr);

                case SemanticKind.FailedExpression:
                    return EvaluateFailedExpression((SemanticFailedExpression) expr);
        }

        throw new Exception($"Unexpected expression type {expr?.Kind}");
    }

    private LiteralValue EvaluateParenExpression(SemanticParenExpression expr)
        => EvaluateExpression(expr.Expression);

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

    private LiteralValue EvaluateBinaryOperation(SemanticBinaryOperation biop)
    {
        var left   = EvaluateExpression(biop.Left);
        var kind   = biop.Operator.Kind;
        var right  = EvaluateExpression(biop.Right);

        bool   _bool;
        double _double;
        string _string;

        if (left.Type is ValType.Unknown || right.Type is ValType.Unknown)
            return UnknownValue.Template;

        if (kind is not (BinaryOperationKind.Equality or BinaryOperationKind.Inequality or BinaryOperationKind.NullishCoalescence)
        && (left is UndefinedValue || right is UndefinedValue))
        {
            string val = "";
            ValType type = biop.Type;
            Span span = biop.Span;
            if (biop.Left is SemanticName L)
            {
                    val  = L.Value;
                    type = L.Type;
                    span = L.Span;
            }
            if (biop.Right is SemanticName R)
            {
                    val  = R.Value;
                    type = R.Type;
                    span = R.Span;
            }

            Except($"Name '{val}' (of type '{type.str()}') not assigned to yet", span);
            return UnknownValue.Template;
        }

        switch (kind)
        {
            case BinaryOperationKind.NullishCoalescence:
                return left.Value is null ? right : left;

            case BinaryOperationKind.Equality:
            case BinaryOperationKind.Inequality:
                _bool =
                      kind == BinaryOperationKind.Equality
                    ?  Equals(left.Value, right.Value)
                    : kind == BinaryOperationKind.Inequality
                    ? !Equals(left.Value, right.Value)
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
                    Except($"Integer too big for bitwise {kind}", biop.Span, ExceptionType.OverflowError);
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
            case BinaryOperationKind.Greater:
            case BinaryOperationKind.GreaterEqual:
            case BinaryOperationKind.Less:
            case BinaryOperationKind.LessEqual:
                _bool =
                      kind == BinaryOperationKind.LAND
                    ? (bool) left.Value && (bool) right.Value

                    : kind == BinaryOperationKind.LOR
                    ? (bool) left.Value || (bool) right.Value


                    :  kind == BinaryOperationKind.Greater
                    ? (double) left.Value > (double) right.Value

                    : kind == BinaryOperationKind.GreaterEqual
                    ? (double) left.Value >= (double) right.Value

                    : kind == BinaryOperationKind.Less
                    ? (double) left.Value < (double) right.Value

                    : kind == BinaryOperationKind.LessEqual
                    ? (double) left.Value <= (double) right.Value
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

                if (double.IsInteger(_double) || double.IsInfinity(_double) || double.IsNaN(_double))
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

        throw new Exception($"Unrecognized binary operation kind: {biop.Operator}");
    }

    private LiteralValue EvaluateTernaryOperation(SemanticTernaryOperation terop)
    {
        var conditionVal = EvaluateExpression(terop.Condition);
        var trueExprVal  = EvaluateExpression(terop.TrueExpression);
        var falseExprVal = EvaluateExpression(terop.FalseExpression);

        if (conditionVal.Type == ValType.Boolean)
            return (bool) conditionVal.Value ? trueExprVal : falseExprVal;

        return UnknownValue.Template;
    }

    private LiteralValue EvaluateAssignExpression(SemanticAssignment aseprx)
    {
        var val = EvaluateExpression(aseprx.Expression);

        if (val.Type is ValType.Unknown)
        {
            if (!Scope.Contains(aseprx.Assignee))
                return val;
        }
        else
            Scope.Assign(aseprx.Assignee, val);

        return Scope.TryResolve(aseprx.Assignee.Value);
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
