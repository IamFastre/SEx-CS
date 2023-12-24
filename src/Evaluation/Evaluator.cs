using SEx.Diagnose;
using SEx.Evaluate.Conversions;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Logic;
using SEx.Generic.Text;
using SEx.Scoping;
using SEx.Scoping.Symbols;
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

                case SemanticKind.FunctionStatement:
                    return EvaluateFunctionStatement((SemanticFunctionStatement) stmt);

                case SemanticKind.IfStatement:
                    return EvaluateIfStatement((SemanticIfStatement) stmt);

                case SemanticKind.WhileStatement:
                    return EvaluateWhileStatement((SemanticWhileStatement) stmt);

                case SemanticKind.ForStatement:
                    return EvaluateForStatement((SemanticForStatement) stmt);

                case SemanticKind.ReturnStatement:
                    return EvaluateReturnStatement((SemanticReturnStatement) stmt);

                default:
                    throw new Exception($"Unexpected statement type {stmt?.Kind}");
        }
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
        }
        else if (ds.Variable.IsConstant)
            Scope.MakeConstant(ds.Variable.Name);

        return VoidValue.Template;
    }

    private LiteralValue EvaluateFunctionStatement(SemanticFunctionStatement fs)
    {
        var val = new FunctionValue(fs.Function.Name, fs.Parameters, fs.ReturnType, fs.Body);
        Scope.Declare(fs.Function, val);
        return VoidValue.Template;
    }

    private LiteralValue EvaluateIfStatement(SemanticIfStatement @is)
    {
        LiteralValue value = VoidValue.Template;
        var conditionVal = EvaluateExpression(@is.Condition);

        if (TypeSymbol.Boolean.Matches(conditionVal.Type))
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

        if (TypeSymbol.Boolean.Matches(conditionVal.Type))
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

        if (expr.Type.IsIterable)
        {
            var iterator = IEnumerableValue.GetIterator(expr);

            if (iterator is null)
                return value;


            foreach (var elem in iterator)
            {
                Scope = new(Scope);
                Scope.Assign(fs.Variable, elem, true);
                value = EvaluateStatement(fs.Body);
                Scope = Scope.Parent!;
            }

        }

        return value;
    }

    private LiteralValue EvaluateReturnStatement(SemanticReturnStatement rs)
        => EvaluateExpression(rs.Expression);

    private LiteralValue EvaluateExpressionStatement(SemanticExpressionStatement es)
        => EvaluateExpression(es.Expression);

    //=====================================================================//
    //=====================================================================//
    //=====================================================================//

    private LiteralValue EvaluateExpression(SemanticExpression? expr)
        => expr is null
        ?  UnknownValue.Template
        :  expr.Kind switch
        {
            SemanticKind.Literal              => EvaluateLiteral((SemanticLiteral) expr),
            SemanticKind.Range                => EvaluateRange((SemanticRange) expr),
            SemanticKind.Name                 => EvaluateName((SemanticName) expr),
            SemanticKind.List                 => EvaluateList((SemanticList) expr),
            SemanticKind.Function             => EvaluateFunctionExpression((SemanticFunction) expr),
            SemanticKind.CallExpression       => EvaluateCallExpression((SemanticCallExpression) expr),
            SemanticKind.IndexingExpression   => EvaluateIndexingExpression((SemanticIndexingExpression) expr),
            SemanticKind.UnaryOperation       => EvaluateUnaryOperation((SemanticUnaryOperation) expr),
            SemanticKind.CountingOperation    => EvaluateCountingOperation((SemanticCountingOperation) expr),
            SemanticKind.ConversionExpression => EvaluateConversionExpression((SemanticConversionExpression) expr),
            SemanticKind.BinaryOperation      => EvaluateBinaryOperation((SemanticBinaryOperation) expr),
            SemanticKind.TernaryOperation     => EvaluateTernaryOperation((SemanticTernaryOperation) expr),
            SemanticKind.AssignExpression     => EvaluateAssignExpression((SemanticAssignment) expr),
            SemanticKind.FailedOperation      => EvaluateFailedExpression((SemanticFailedOperation) expr),
            SemanticKind.FailedExpression     => UnknownValue.Template,

            _ => throw new Exception($"Unrecognized expression kind {expr.Kind}")
        };

    private LiteralValue EvaluateLiteral(SemanticLiteral expr)
    {
        if (TypeSymbol.Null.Matches(expr.Type))
            return ParseNull(expr);

        if (TypeSymbol.Boolean.Matches(expr.Type))
            return ParseBool(expr);

        if (TypeSymbol.Integer.Matches(expr.Type))
            return ParseInt(expr);

        if (TypeSymbol.Float.Matches(expr.Type))
            return ParseFloat(expr);

        if (TypeSymbol.Number.Matches(expr.Type))
            return ParseNumber(expr);

        if (TypeSymbol.Char.Matches(expr.Type))
            return ParseChar(expr);

        if (TypeSymbol.String.Matches(expr.Type))
            return ParseString(expr);

        return UnknownValue.Template;
    }

    private LiteralValue EvaluateRange(SemanticRange r)
    {
        LiteralValue value = UnknownValue.Template;

        var start = EvaluateExpression(r.Start);
        var end   = EvaluateExpression(r.End);
        var step  = r.Step is null
                  ? new IntegerValue(1D)
                  : EvaluateExpression(r.Step);


        if (TypeSymbol.Number.Matches(start.Type)
        &&  TypeSymbol.Number.Matches(end.Type)
        &&  TypeSymbol.Number.Matches(step.Type))
        {
            value = new RangeValue((NumberValue) start, (NumberValue) end, (NumberValue) step);

            if (((RangeValue) value).Length is null)
            {
                Diagnostics.Report.BadRangeDirection(r.Span);
                return UndefinedValue.New(TypeSymbol.Range);
            }

            if ((double)((RangeValue) value).Step.Value == 0)
            {
                Diagnostics.Report.RangeStepIsZero(r.Span);
                return UndefinedValue.New(TypeSymbol.Range);
            }
        }

        return value;
    }

    private LiteralValue EvaluateName(SemanticName n)
    {
        if (!Scope.TryResolve(n.Symbol, out var value))
        {
            Diagnostics.Report.UndefinedName(n.Symbol.Name, n.Span);
            return UnknownValue.Template;
        }

        if (value.IsNull)
        {
            Diagnostics.Report.NullReference(n.Span);
            return UnknownValue.Template;
        }

        if (!value.IsDefined)
        {
            Diagnostics.Report.UseOfUndefined(n.Symbol.Name, n.Span);
            return UnknownValue.Template;
        }

        return value;
    }

    private LiteralValue EvaluateList(SemanticList ll)
    {
        List<LiteralValue> values = new();

        foreach (var statement in ll.Elements)
        {
            var value = EvaluateExpression(statement);
            if (value.IsKnown)
                values.Add(value);
            else
                return UndefinedValue.New(ll.Type);
        }

        var type = values.Count > 0 ? values[0].Type : ll.ElementType; 

        return new ListValue(values, type);
    }

    private FunctionValue EvaluateFunctionExpression(SemanticFunction fe)
        => new(CONSTS.EMPTY, fe.Parameters, ((GenericTypeSymbol) fe.Type).Parameters[0], fe.Body);

    private LiteralValue EvaluateCallExpression(SemanticCallExpression fc)
    {
        var val = EvaluateExpression(fc.Function);
        if (val is FunctionValue func)
        {
            var args = fc.Arguments.Select(EvaluateExpression).ToArray();
            if (func.IsBuiltin)
                return BuiltIn.Backend.Evaluate(func, args);

            Scope = new(Scope);

            for (int i = 0; i < fc.Arguments.Length; i++)
                Scope.Declare(func.Parameters[i],
                            EvaluateExpression(fc.Arguments[i]));

            var value = EvaluateStatement(func.Body);
            Scope = Scope.Parent!;

            return value;
        }

        return UndefinedValue.New(fc.Type);
    }

    private LiteralValue EvaluateIndexingExpression(SemanticIndexingExpression ie)
    {
        var iterable = EvaluateExpression(ie.Iterable);
        var index    = EvaluateExpression(ie.Index);
        var elem     = IIndexableValue.GetIndexed(iterable, index);

        if (elem is null)
            Diagnostics.Report.IndexOutOfBoundary(ie.Index.Span);

        return elem ?? UndefinedValue.New(iterable.Type.ElementType);
    }

    private LiteralValue EvaluateUnaryOperation(SemanticUnaryOperation uop)
    {
        var operand = EvaluateExpression(uop.Operand);
        if (!operand.IsKnown)
            return UndefinedValue.New(uop.Type);

        double _double;

        switch (uop.OperationKind)
        {
            case UnaryOperationKind.Identity:
                return operand;
            
            case UnaryOperationKind.Negation:
                _double = -(double) operand.Value;

                if (TypeSymbol.Integer.Matches(operand.Type))
                    return new IntegerValue(_double);
                else
                    return new FloatValue(_double);
            
            case UnaryOperationKind.BitwiseComplement:
                return new IntegerValue(-(double) operand.Value - 1);

            case UnaryOperationKind.Complement:
                return new BoolValue(!(bool) operand.Value);
        }

        throw new Exception($"Unrecognized unary operation kind: {uop.OperationKind}");
    }

    private LiteralValue EvaluateCountingOperation(SemanticCountingOperation co)
    {
        var name = EvaluateName(co.Name);
        var kind = co.OperationKind;

        if (!name.IsKnown)
            return UnknownValue.Template;

        double _double = TypeSymbol.Char.Matches(co.Name.Type)
                       ? (char)   name.Value
                       : (double) name.Value;

        _double
            = kind is CountingKind.IncrementAfter or CountingKind.IncrementBefore
            ? _double + 1D

            : kind is CountingKind.DecrementAfter or CountingKind.DecrementBefore
            ? _double - 1D

            : throw new Exception("This shouldn't occur");

        LiteralValue value;

        if (TypeSymbol.Char.Matches(name.Type))
            value = new CharValue((char) _double);

        else if (TypeSymbol.Integer.Matches(name.Type))
            value = new IntegerValue(_double);

        else
            value = new FloatValue(_double);

        Scope.Assign(co.Name.Symbol, value);

        return kind is CountingKind.IncrementAfter or CountingKind.DecrementAfter
             ? name
             : value;
    }

    private LiteralValue EvaluateConversionExpression(SemanticConversionExpression ce)
    {
        var value = EvaluateExpression(ce.Expression);
        var after = Converter.Convert(ce.ConversionKind, value, ce.Target);

        if (!after.IsDefined && value.IsKnown)
            Diagnostics.Report.CannotConvert(value.Type.ToString(), ce.Target.ToString(), ce.Span);

        return after;
    }

    private LiteralValue EvaluateBinaryOperation(SemanticBinaryOperation biop)
    {
        var left   = EvaluateExpression(biop.Left);
        var kind   = biop.Operator.Kind;
        var right  = EvaluateExpression(biop.Right);

        if (!left.IsKnown || !right.IsKnown)
            return UnknownValue.Template;

        bool   _bool;
        double _double;
        string _string;

        switch (kind)
        {
            case BinaryOperationKind.NullishCoalescence:
                return left.Value is null ? right : left;

            case BinaryOperationKind.Equality:
            case BinaryOperationKind.Inequality:
                _bool =
                      kind == BinaryOperationKind.Equality
                    ?  left.Equals(right)
                    : kind == BinaryOperationKind.Inequality
                    ? !left.Equals(right)
                    : throw new Exception("This shouldn't occur");

                return new BoolValue(_bool);

            case BinaryOperationKind.BitwiseAND:
            case BinaryOperationKind.BitwiseOR:
            case BinaryOperationKind.BitwiseXOR:
                Int128 _i1, _i2;
                if (TypeSymbol.Integer.Matches(left.Type) && (double) left.Value > (double) Int128.MaxValue)
                {
                    Except($"Integer too big for bitwise {kind}", biop.Span, ExceptionType.OverflowError);
                    return UnknownValue.Template;
                }
                else if (left is BoolValue b)
                    _i1  = b.ToInt();
                else
                    _i1 = Int128.Parse(left.Value.ToString() ?? "");

                if (TypeSymbol.Integer.Matches(right.Type) && (double) right.Value > (double) Int128.MaxValue)
                {
                    Except($"Integer too big for bitwise {kind}", biop.Span, ExceptionType.OverflowError);
                    return UnknownValue.Template;
                }
                else if (right is BoolValue b)
                    _i2  = b.ToInt();
                else
                    _i2 = Int128.Parse(right.Value.ToString() ?? "");

                _double = (double)
                    ( kind == BinaryOperationKind.BitwiseAND
                    ?  _i1 & _i2

                    : kind == BinaryOperationKind.BitwiseOR
                    ?  _i1 | _i2

                    : kind == BinaryOperationKind.BitwiseXOR
                    ?  _i1 ^  _i2

                    : throw new Exception("This shouldn't occur"));

                if (left is IntegerValue || right is IntegerValue)
                    return new IntegerValue(_double);
                return new BoolValue(_double != 0D);

            case BinaryOperationKind.LogicalAND:
            case BinaryOperationKind.LogicalOR:
            case BinaryOperationKind.Greater:
            case BinaryOperationKind.GreaterEqual:
            case BinaryOperationKind.Less:
            case BinaryOperationKind.LessEqual:
                _bool =
                      kind == BinaryOperationKind.LogicalAND
                    ? (bool) left.Value && (bool) right.Value

                    : kind == BinaryOperationKind.LogicalOR
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

                if (TypeSymbol.Integer.Matches(biop.Type))
                    return new IntegerValue(_double);
                else
                    return new FloatValue(_double);

            //=====================================================================//

            case BinaryOperationKind.RangeInclusion:
                _bool = ((RangeValue) right).Contains((NumberValue) left);
                return new BoolValue(_bool);

            //=====================================================================//

            case BinaryOperationKind.CharAddition:
            case BinaryOperationKind.CharSubtraction:
                var aos = (double) (TypeSymbol.Integer.Matches(left.Type) ? left.Value : right.Value);
                var cr1 = ( char ) (TypeSymbol.Char.Matches(left.Type)    ? left.Value : right.Value);

                _double = kind == BinaryOperationKind.CharAddition
                    ? aos + cr1
                    : aos - cr1;

                return new CharValue((char) (int) Math.Abs(_double));


            //=====================================================================//

            case BinaryOperationKind.StringConcatenation:
                _string = left.GetString() + right.GetString();
                return new StringValue(_string);

            case BinaryOperationKind.StringMultiplication:
                var num = (double) (TypeSymbol.Integer.Matches(left.Type) ? left.Value : right.Value);
                var str = (string) (TypeSymbol.String.Matches(left.Type)  ? left.Value : right.Value);

                _string = string.Empty;

                if (num < 0)
                    str = new(str.Reverse().ToArray());

                for (int i = 0; i < Math.Abs(num); i++)
                    _string += str;

                return new StringValue(_string);

            case BinaryOperationKind.StringInclusion:
                if (left.Type.ID == TypeID.String)
                    _bool = ((StringValue) right).Contains((StringValue) left);
                else
                    _bool = ((StringValue) right).Contains((CharValue) left);
    
                return new BoolValue(_bool);

            //=====================================================================//

            case BinaryOperationKind.ListConcatenation:
                ListValue _l1, _l2;
                (_l1, _l2) = ((ListValue) left, (ListValue) right);
                return _l1.Concat(_l2);

            case BinaryOperationKind.ListInclusion:
                _bool = ((List<LiteralValue>) right.Value).Any(left.Equals);
                return new BoolValue(_bool);
        }

        throw new Exception($"Unrecognized binary operation kind: {biop.Operator}");
    }

    private LiteralValue EvaluateTernaryOperation(SemanticTernaryOperation terop)
    {
        var conditionVal = EvaluateExpression(terop.Condition);
        var trueExprVal  = EvaluateExpression(terop.TrueExpression);
        var falseExprVal = EvaluateExpression(terop.FalseExpression);

        if (TypeSymbol.Boolean.Matches(conditionVal.Type))
            return (bool) conditionVal.Value ? trueExprVal : falseExprVal;

        return UnknownValue.Template;
    }

    private LiteralValue EvaluateAssignExpression(SemanticAssignment aseprx)
    {
        var val = EvaluateExpression(aseprx.Expression);

        if (Scope.TryResolve(aseprx.Assignee.Symbol, out var output) && (!aseprx.Assignee.Symbol.IsConstant))
            Scope.Assign(aseprx.Assignee.Symbol, output = val);

        return output.Type.IsKnown ? output : val;
    }

    private LiteralValue EvaluateFailedExpression(SemanticFailedOperation fe)
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
        return UndefinedValue.New(TypeSymbol.Boolean);
    }

    private LiteralValue ParseNumber(SemanticLiteral literal)
    {
        double num;
        try
        {
            num = double.Parse(literal.Value);
            return NumberValue.Get(num);
        }
        catch
        {
            Except($"Error ocurred while parsing number", literal.Span!, ExceptionType.InternalError);
        }

        return UndefinedValue.New(TypeSymbol.Number);
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

        return UndefinedValue.New(TypeSymbol.Integer);
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

        return UndefinedValue.New(TypeSymbol.Float);
    }

    private LiteralValue ParseChar(SemanticLiteral literal)
    {
        string? str = Prepare(literal);

        if (str is null)
            return UndefinedValue.New(TypeSymbol.Char);

        else if (!char.TryParse(str, out char chr))
            Except($"Invalid character string", literal.Span!);

        else
            return new CharValue(chr);

        return UndefinedValue.New(TypeSymbol.Char);
    }

    private LiteralValue ParseString(SemanticLiteral literal)
    {
        string? str = Prepare(literal);
        if (str is not null)
            return new StringValue(str);

        return UndefinedValue.New(TypeSymbol.String);
    }

    //=====================================================================//
    //=====================================================================//
    //=====================================================================//

    private string? Prepare(SemanticLiteral node)
    {
        try { return node.Value.Unescape()[1..^1]; }
        catch { Except($"Invalid escape sequence", node.Span!, ExceptionType.StringParseError); }

        return null;
    }
}
