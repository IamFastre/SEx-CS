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
    public Diagnostics              Diagnostics  { get; }
    public SemanticProgramStatement SemanticTree { get; }
    public Scope                    Scope        { get; protected set; }
    public LiteralValue             Value        { get; protected set; }

    public Evaluator(SemanticProgramStatement stmt, Scope? scope = null, Diagnostics? diagnostics = null)
    {
        SemanticTree = stmt;
        Scope        = scope       ?? new();
        Diagnostics  = diagnostics ?? new();
        Value        = UnknownValue.Template;
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.SyntaxError)
        => Diagnostics.Add(type, message, span);

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

                case SemanticKind.WhileStatement:
                    return EvaluateWhileStatement((SemanticWhileStatement) stmt);

                case SemanticKind.ForStatement:
                    return EvaluateForStatement((SemanticForStatement) stmt);

                default:
                    throw new Exception($"Unexpected statement type {stmt?.Kind}");
        }
    }

    private LiteralValue EvaluateIfStatement(SemanticIfStatement @is)
    {
        LiteralValue value = VoidValue.Template;
        var conditionVal = EvaluateExpression(@is.Condition);

        if (conditionVal.Type == ValType.Boolean)
        {
            if ((bool) conditionVal.Value)
                value = EvaluateStatement(@is.Then);
            else
                if (@is.ElseClause is not null)
                    value = EvaluateStatement(@is.ElseClause.Body);
        }

        return value;
    }

    private LiteralValue EvaluateWhileStatement(SemanticWhileStatement ws)
    {
        LiteralValue value = VoidValue.Template;
        var conditionVal = EvaluateExpression(ws.Condition);

        if (conditionVal.Type == ValType.Boolean)
        {
            if ((bool) conditionVal.Value)
                while ((bool) EvaluateExpression(ws.Condition).Value)
                    value = EvaluateStatement(ws.Body);
            else
                if (ws.ElseClause is not null)
                    value = EvaluateStatement(ws.ElseClause.Body);
        }

        return value;
    }

    private LiteralValue EvaluateForStatement(SemanticForStatement fs)
    {
        LiteralValue value = VoidValue.Template;
        var expr           = EvaluateExpression(fs.Iterable);

        if (ValType.Iterable.HasFlag(expr.Type))
        {
            var iterableVal = (IIterableValue) expr;
            var len         = (double)(iterableVal.Length?.Value ?? 0);

            Scope = new(Scope);

            for (int i = 0; i < len; i++)
            {
                var elem = IIterableValue.GetElement((LiteralValue) iterableVal, new IntegerValue(i));

                if (elem is null)
                    break;

                if (i == 0)
                    Scope.Declare(fs.Variable, elem);
                else
                    Scope.Assign(fs.Variable, elem);

                value = EvaluateStatement(fs.Body);
            }

            Scope = Scope.Parent!;
        }

        return value;
    }

    private LiteralValue EvaluateBlockStatement(SemanticBlockStatement bs)
    {
        LiteralValue lastValue = VoidValue.Template;
        foreach (var statement in bs.Body)
            lastValue = EvaluateStatement(statement);

        return lastValue;
    }

    private LiteralValue EvaluateDeclarationStatement(SemanticDeclarationStatement ds)
    {
        var value = ds.Expression is null
                  ? UndefinedValue.New(ds.Variable.Type)
                  : EvaluateExpression(ds.Expression);

        if (!Scope.TryResolve(ds.Variable, out _))
        {
            if ((ds.Variable.Type, value.Type).IsAssignable())
                Scope.Declare(ds.Variable, value);
            else
                Scope.Declare(ds.Variable, UndefinedValue.New(ds.Variable.Type));
        }
        else if (ds.Variable.IsConstant && ds.Expression is null)
            Scope.MakeConstant(ds.Variable.Name);

        return VoidValue.Template;
    }

    private LiteralValue EvaluateExpressionStatement(SemanticExpressionStatement es)
        => EvaluateExpression(es.Expression);

    //=====================================================================//
    //=====================================================================//
    //=====================================================================//

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

                case SemanticKind.Range:
                    return EvaluateRange((SemanticRange) expr);

                case SemanticKind.Name:
                    return EvaluateVariable((SemanticVariable) expr);

                case SemanticKind.List:
                    return EvaluateArray((SemanticList) expr);

                case SemanticKind.IndexingExpression:
                    return EvaluateIndexingExpression((SemanticIndexingExpression) expr);

                case SemanticKind.ParenExpression:
                    return EvaluateParenExpression((SemanticParenExpression) expr);

                case SemanticKind.UnaryOperation:
                    return EvaluateUnaryOperation((SemanticUnaryOperation) expr);

                case SemanticKind.CountingOperation:
                    return EvaluateCountingOperation((SemanticCountingOperation) expr);

                case SemanticKind.BinaryOperation:
                    return EvaluateBinaryOperation((SemanticBinaryOperation) expr);

                case SemanticKind.TernaryOperation:
                    return EvaluateTernaryOperation((SemanticTernaryOperation) expr);

                case SemanticKind.AssignExpression:
                    return EvaluateAssignExpression((SemanticAssignment) expr);

                case SemanticKind.FailedExpression:
                    return UnknownValue.Template;

                case SemanticKind.FailedExpressions:
                    return EvaluateFailedExpression((SemanticFailedExpressions) expr);
        }

        throw new Exception($"Unexpected expression type {expr?.Kind}");
    }

    private LiteralValue EvaluateParenExpression(SemanticParenExpression pe)
        => EvaluateExpression(pe.Expression);

    private LiteralValue EvaluateRange(SemanticRange r)
    {
        LiteralValue value = UnknownValue.Template;

        var start = EvaluateExpression(r.Start);
        var end   = EvaluateExpression(r.End);
        var step  = r.Step is null
                  ? new IntegerValue(1D)
                  : EvaluateExpression(r.Step);


        if (ValType.Number.HasFlag(start.Type)
        &&  ValType.Number.HasFlag(end.Type)
        &&  ValType.Number.HasFlag(step.Type))
        {
            value = new RangeValue((NumberValue) start, (NumberValue) end, (NumberValue) step);
            if (((RangeValue) value).Length is null)
            {
                Except($"Range end point and step direction don't match", r.Span, ExceptionType.MathError);
                value = UnknownValue.Template;
            }
        }


        return value;
    }

    private LiteralValue EvaluateVariable(SemanticVariable n)
    {
        if (!Scope.TryResolve(n.Symbol, out var value))
            Diagnostics.Report.UndefinedVariable(n.Symbol.Name, n.Span);

        return value;
    }

    private LiteralValue EvaluateArray(SemanticList ll)
    {
        List<LiteralValue> values = new();

        foreach (var statement in ll.Elements)
            values.Add(EvaluateExpression(statement));

        var type = values.Count > 0 ? values[0].Type : ll.ElementType; 

        return new ListValue(values, type);
    }

    private LiteralValue EvaluateIndexingExpression(SemanticIndexingExpression ie)
    {
        var iterable = EvaluateExpression(ie.Iterable);
        var index    = EvaluateExpression(ie.Index);
        var elem     = IIterableValue.GetElement(iterable, index);

        if (elem is null)
            Except($"Index is out of boundary", ie.Index.Span);

        else if (elem.Type is ValType.Unknown)
            Except($"Can't perform indexing on '{iterable.Type.str()}' with '{index.Type.str()}'", ie.Index.Span);

        return elem ?? UnknownValue.Template;
    }

    private LiteralValue EvaluateUnaryOperation(SemanticUnaryOperation uop)
    {
        var operand = EvaluateExpression(uop.Operand);

        double _double;

        switch (uop.OperationKind)
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

        throw new Exception($"Unrecognized unary operation kind: {uop.OperationKind}");
    }

    private LiteralValue EvaluateCountingOperation(SemanticCountingOperation co)
    {
        var name = EvaluateVariable(co.Name);
        var kind = co.OperationKind;

        double _double
            = kind is CountingKind.IncrementAfter or CountingKind.IncrementBefore
            ? (double) name.Value + 1D

            : kind is CountingKind.DecrementAfter or CountingKind.DecrementBefore
            ? (double) name.Value - 1D

            : throw new Exception("This shouldn't occur");

        LiteralValue value;
        if (name.Type is ValType.Integer)
            value = new IntegerValue(_double);
        else
            value = new FloatValue(_double);

        Scope.Assign(co.Name.Symbol, value);

        return kind is CountingKind.IncrementAfter or CountingKind.DecrementAfter
             ? value
             : name;
    }

    private LiteralValue EvaluateBinaryOperation(SemanticBinaryOperation biop)
    {
        var left   = EvaluateExpression(biop.Left);
        var kind   = biop.Operator.Kind;
        var right  = EvaluateExpression(biop.Right);

        bool   _bool;
        double _double;
        string _string;
        // List<LiteralValue> _list;

        if (!left.IsKnown || !right.IsKnown)
            return UnknownValue.Template;

        if (!left.IsDefined)
            return UseOfUndefined((SemanticVariable) biop.Left);

        if (!right.IsDefined)
            return UseOfUndefined((SemanticVariable) biop.Right);

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
                Int128 _i1, _i2;
                if (left.Type is ValType.Integer && (double) left.Value > (double) Int128.MaxValue)
                {
                    Except($"Integer too big for bitwise {kind}", biop.Span, ExceptionType.OverflowError);
                    return UnknownValue.Template;
                }
                else if (left is BoolValue b)
                    _i1  = b.ToInt();
                else
                    _i1 = Int128.Parse(left.Value.ToString() ?? "");

                if (right.Type is ValType.Integer && (double) right.Value > (double) Int128.MaxValue)
                {
                    Except($"Integer too big for bitwise {kind}", biop.Span, ExceptionType.OverflowError);
                    return UnknownValue.Template;
                }
                else if (right is BoolValue b)
                    _i2  = b.ToInt();
                else
                    _i2 = Int128.Parse(right.Value.ToString() ?? "");

                _double = (double)
                    ( kind == BinaryOperationKind.AND
                    ?  _i1 & _i2

                    : kind == BinaryOperationKind.OR
                    ?  _i1 | _i2

                    : kind == BinaryOperationKind.XOR
                    ?  _i1 ^  _i2

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

            case BinaryOperationKind.RangeInclusion:
                _bool = ((RangeValue) right).Contains(left);
                return new BoolValue(_bool);

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

            case BinaryOperationKind.StringInclusion:
                _bool = ((StringValue) right).Contains(left);
                return new BoolValue(_bool);

            //=====================================================================//

            case BinaryOperationKind.ListConcatenation:
                ListValue _l1, _l2;
                (_l1, _l2) = ((ListValue) left, (ListValue) right);

                if (!(_l1.ElementType, _l2.ElementType).IsAssignable(true))
                {
                    Except($"Cannot concatenate list of type '{_l1.ElementType.str()}' to '{_l2.ElementType.str()}'",
                           biop.Span, ExceptionType.TypeError);
                    return UnknownValue.Template;
                }

                return _l1.Concat(_l2);

            case BinaryOperationKind.ListInclusion:
                _bool = ((List<LiteralValue>) right.Value).Any((LiteralValue val) => Equals(val.Value, left.Value));
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

        if (Scope.TryResolve(aseprx.Assignee, out var output) && (!aseprx.Assignee.IsConstant))
            Scope.Assign(aseprx.Assignee, val);

        return val.Type is ValType.Unknown ? output : val;
    }

    private LiteralValue EvaluateFailedExpression(SemanticFailedExpressions fe)
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

    //=====================================================================//
    //=====================================================================//
    //=====================================================================//

    private LiteralValue UseOfUndefined(SemanticVariable n)
    {
        Except($"Name '{n.Symbol.Name}' (of type '{n.Type.str()}') not assigned to yet", n.Span);
        return UnknownValue.Template;
    }

}
