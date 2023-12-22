using SEx.AST;
using SEx.Diagnose;
using SEx.Scoping;
using SEx.Scoping.Symbols;
using SEx.Generic.Text;
using SEx.Parse;
using System.Collections.Immutable;

namespace SEx.Semantics;

internal sealed class Analyzer
{
    public Diagnostics                Diagnostics { get; }
    public ProgramStatement           SimpleTree  { get; }
    public SemanticScope              Scope       { get; private set; }
    public SemanticProgramStatement?  Tree        { get; private set; }

    public Analyzer(ProgramStatement stmt, SemanticScope? scope = null, Diagnostics? diagnostics = null)
    {
        SimpleTree  = stmt;
        Scope       = scope       ?? new();
        Diagnostics = diagnostics ?? new();
    }

    private void Except(string message,
                        Span span,
                        ExceptionType type = ExceptionType.TypeError)
        => Diagnostics.Add(type, message, span);

    public SemanticProgramStatement Analyze()
        => Tree = BindProgram(SimpleTree);

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

            case NodeKind.FunctionStatement:
                return BindFunctionStatement((FunctionStatement) stmt);

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

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        List<SemanticStatement> statements = new();

        foreach (var statement in bs.Body)
            statements.Add(BindStatement(statement));

        return new(statements.ToArray(), bs.Span);
    }

    private SemanticDeclarationStatement BindDeclarationStatement(DeclarationStatement ds)
    {
        var expr = BindExpression(ds.Expression);
        var hint = ds.TypeClause is null ? expr.Type : BindTypeClause(ds.TypeClause);
        NameSymbol var  = new NameSymbol(ds.Variable.Value, hint, ds.IsConstant);

        if (ds.IsConstant && !expr.Type.IsKnown)
            var = BindMakeConstant(ds) ?? var;

        else if (expr.Type.IsKnown)
        {
            if (!(hint, expr.Type).IsAssignable())
                Diagnostics.Report.TypesDoNotMatch(hint.ToString(), expr.Type.ToString(), ds.Span);

            else if (!TypeSymbol.Any.Matches(expr.Type))
                Diagnostics.Report.CannotAssignType(expr.Type.ToString(), ds.Expression!.Span);

            else if (!Scope.TryDeclare(var))
                    Diagnostics.Report.AlreadyDefined(var.Name, ds.Variable.Span);
        }

        return new(var, ds.Variable.Span, expr, ds.Span);
    }

    private NameSymbol? BindMakeConstant(DeclarationStatement ds)
    {
        if (Scope.TryResolve(ds.Variable.Value, out var symbol))
        {
            if (ds.TypeClause is not null)
                Diagnostics.Report.UselessTypeAdded(ds.TypeClause.Span);

            else if (symbol!.IsConstant)
                Diagnostics.Report.AlreadyConstant(symbol.Name, ds.Variable.Span);

            else
            {
                var sym = Scope.Symbols[symbol.Name];
                sym.MakeConstant();
                return sym;
            }
        }
        else
            Diagnostics.Report.ValuelessConstant(ds.Variable.Value, ds.Variable.Span);

        return null;
    }

    private SemanticFunctionStatement BindFunctionStatement(FunctionStatement fs)
    {
        var returnType = fs.Hint is not null ? BindTypeClause(fs.Hint) : TypeSymbol.Void;
        var parameters = fs.Parameters.Nodes.Select(BindParameter).ToArray();
        var type       = TypeSymbol.Function(returnType, parameters.Select(p => p.Type).ToArray());
        var name       = new NameSymbol(fs.Name.Value, type, fs.IsConstant);

        Scope = new(Scope);
        foreach (var p in parameters)
            Scope.TryDeclare(p);
        var body = BindStatement(fs.Body);
        Scope = Scope.Parent!;

        if (returnType.IsKnown)
            if (!Scope.TryDeclare(name))
                Diagnostics.Report.AlreadyDefined(name.Name, fs.Name.Span);

        return new(name, parameters, returnType, body, fs.Span);
    }

    private SemanticIfStatement BindIfStatement(IfStatement @is)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(@is.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(@is.Then);

        if (@is.ElseClause is not null)
            elseClause = BindElseClause(@is.ElseClause);

        return new(condition, thenStmt, @is.Span, elseClause);
    }

    private SemanticWhileStatement BindWhileStatement(WhileStatement ws)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(ws.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(ws.Body);

        if (ws.ElseClause is not null)
            elseClause = BindElseClause(ws.ElseClause);

        return new(condition, thenStmt, ws.Span, elseClause);
    }

    private SemanticElseClause BindElseClause(ElseClause ec)
    {
        var elseStmt = BindStatement(ec.Body);
        return new(elseStmt, ec.Span);
    }

    private SemanticForStatement BindForStatement(ForStatement fs)
    {
        var iterable = BindExpression(fs.Iterable);
        var elemType = iterable.Type.ElementType;

        if (elemType is null)
            if (iterable.Type.IsKnown)
                Diagnostics.Report.CannotIterate(iterable.Type.ToString(), iterable.Span);

        Scope = new(Scope);

        var variable = DeclareVariable(fs.Variable, elemType ?? TypeSymbol.Unknown, true);
        var body     = BindStatement(fs.Body);

        Scope = Scope.Parent!;

        return new(variable, iterable, body, fs.Span);
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement es)
    {
        var expr = BindExpression(es.Expression);
        return new(expr);
    }

    //=====================================================================//
    
    private NameSymbol DeclareVariable(NameLiteral n, TypeSymbol type, bool isConst = false)
    {
        var symbol = new NameSymbol(n.Value, type, isConst);
        Scope.Symbols.Add(n.Value, symbol);
        return symbol;
    }

    private TypeSymbol[] BindTypeClauses(TypeClause[] tcs)
    {
        List<TypeSymbol> types = new();
        foreach (var tc in tcs)
        {
            var type = BindTypeClause(tc);
            if (!TypeSymbol.Unknown.Matches(type))
                types.Add(type);
        }
        
        return types.ToArray();
    }

    private TypeSymbol BindTypeClause(TypeClause tc)
    {
        var type = tc.Kind == NodeKind.TypeClause
                 ? TypeSymbol.GetTypeByString(tc.Type.Value)
                 : GenericTypeSymbol.GetTypeByString(tc.Type.Value, BindTypeClauses(((GenericTypeClause) tc).Parameters));

        if (type is null || !type.IsKnown)
        {
            Diagnostics.Report.InvalidTypeClause(tc.Span);
            return TypeSymbol.Unknown;
        }

        for (int i = 0; i < tc.ListDimension; i++)
            type = TypeSymbol.TypedList(type);
        
        return type;
    }

    private NameSymbol BindParameter(ParameterClause pc)
        => new(pc.Name.Value, BindTypeClause(pc.Type));

    private NameSymbol? GetNameSymbol(NameLiteral n)
    {
        var result = Scope.Resolve(n.Value);

        if (result is not null)
            return result;

        Diagnostics.Report.UndefinedName(n.Value, n.Span);
        return null;
    }

    //=====================================================================//

    private SemanticExpression BindExpression(Expression expr, TypeSymbol expected)
    {
        var val = BindExpression(expr);
        if (!expected.Matches(val.Type))
            if (val.Type.IsKnown)
                Diagnostics.Report.TypeExpected(expected.ToString(), val.Type.ToString(), expr.Span);

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

            case NodeKind.CallExpression:
                return BindCallExpression((CallExpression) expr);

            case NodeKind.ParenthesizedExpression:
                return BindParenExpression((ParenthesizedExpression) expr);

            case NodeKind.IndexingExpression:
                return BindIndexingExpression((IndexingExpression) expr);

            case NodeKind.UnaryOperation:
                return BindUnaryOperation((UnaryOperation) expr);

            case NodeKind.CountingOperation:
                return BindCountingOperation((CountingOperation) expr);

            case NodeKind.ConversionExpression:
                return BindConversionExpression((ConversionExpression) expr);

            case NodeKind.BinaryOperation:
                return BindBinaryOperation((BinaryOperation) expr);

            case NodeKind.TernaryOperation:
                return BindTernaryOperation((TernaryOperation) expr);

            case NodeKind.AssignmentExpression:
                return BindAssignExpression((AssignmentExpression) expr);

            default:
                throw new Exception($"Unrecognized expression kind: {expr.Kind}");
        }
    }

    private SemanticLiteral BindLiteral(Literal literal)
        => new(literal);

    private SemanticRange BindRange(RangeLiteral r)
    {
        var start = BindExpression(r.Start, TypeSymbol.Number);
        var end   = BindExpression(r.End, TypeSymbol.Number);
        var step  = r.Step is null ? null : BindExpression(r.Step, TypeSymbol.Number);

        return new(start, end, step);
    }

    private SemanticExpression BindName(NameLiteral n)
    {
        var symbol = GetNameSymbol(n);

        if (symbol is not null)
            return new SemanticName(symbol, n.Span);

        return new SemanticFailedExpression(n.Span);
    }

    private SemanticExpression BindList(ListLiteral ll)
    {
        if (ll.Elements.Nodes.Length > 0)
        {
            List<SemanticExpression> expressions = new();
            var arRef = BindExpression((Expression) ll.Elements.Nodes.First());
            TypeSymbol type = arRef.Type;
            expressions.Add(arRef);

            foreach (var elem in ll.Elements.Nodes[1..])
            {
                var expr = BindExpression((Expression) elem);

                if (type.Matches(expr.Type))
                    expressions.Add(expr);
                else if (expr.Type.Matches(type))
                {
                    type = expr.Type;
                    expressions.Add(expr);
                }
                else
                    if (type.IsKnown && expr.Type.IsKnown)
                        Diagnostics.Report.HeteroList(type.ToString(), expr.Type.ToString(), ll.Span);
            }

            if (ll.Elements.Nodes.Length != expressions.Count)
                return new SemanticFailedOperation(expressions.ToArray(), ll.Span);

            return new SemanticList(expressions.ToArray(), type, ll.Span);
        }

        return new SemanticList(Array.Empty<SemanticExpression>(), TypeSymbol.Any, ll.Span);
    }

    private SemanticExpression BindCallExpression(CallExpression fce)
    {
        var func = BindExpression(fce.Function);

        if (func.Type.IsCallable)
        {
            var fs = (GenericTypeSymbol) func.Type;
            var funcParams = fs.Parameters[1..];

            if (funcParams.Length != fce.Arguments.Nodes.Length)
            {
                Diagnostics.Report.InvalidArgumentCount(fs.ToString(), funcParams.Length, fce.Arguments.Nodes.Length, fce.Span);
                return new SemanticFailedExpression(fce.Span);
            }

            var args = BindSeparatedExpression(fce.Arguments);
            var faulty = false;
            for (int i = 0; i < args.Length; i++)
            {
                var paramType = funcParams[i];
                var argType   = args[i].Type;

                if (!paramType.Matches(argType))
                {
                    if (argType.IsKnown)
                        Diagnostics.Report.TypesDoNotMatch(paramType.ToString(), argType.ToString(), args[i].Span);
                    faulty = true;
                }
            }

            if (faulty)
                return new SemanticFailedExpression(fce.Span);

            return new SemanticCallExpression(func, fs.Parameters[0], args, fce.Span);
        }
        else if (func is null)
            return new SemanticFailedExpression(fce.Span);

        if (func.Type.IsKnown)
            Diagnostics.Report.NotCallable(func.Type.ToString(), fce.Function.Span);
        return new SemanticFailedExpression(fce.Span);
    }

    private SemanticExpression[] BindSeparatedExpression(SeparatedClause<Expression> sc)
    {
        var exprs = ImmutableArray.CreateBuilder<SemanticExpression>();
        foreach (var expr in sc.Nodes)
            exprs.Add(BindExpression(expr));

        return exprs.ToArray();
    }

    private SemanticExpression BindParenExpression(ParenthesizedExpression pe)
    {
        var expr = pe.Expression is null
                 ? new SemanticFailedExpression(pe.Span)
                 : BindExpression(pe.Expression);

        return expr;
    }

    private SemanticExpression BindIndexingExpression(IndexingExpression ie)
    {
        var iterable    = BindExpression(ie.Iterable);
        var index       = BindExpression(ie.Index);
        var elementType = iterable.Type.GetIndexReturn(index.Type);

        if (elementType is null)
        {
            Diagnostics.Report.CannotIndex(iterable.Type.ToString(), ie.Iterable.Span);
            return new SemanticFailedOperation(new[] { iterable, index });
        }

        if (!elementType.IsKnown)
        {
            Diagnostics.Report.CannotIndexWithType(iterable.Type.ToString(), index.Type.ToString(), ie.Iterable.Span);
            return new SemanticFailedOperation(new[] { iterable, index });
        }

        return new SemanticIndexingExpression(iterable, index, elementType, ie.Span);
    }

    private SemanticExpression BindUnaryOperation(UnaryOperation uop)
    {
        var operand = BindExpression(uop.Operand);
        var opKind  = SemanticUnaryOperation.GetOperationKind(uop.Operator.Kind, operand.Type.ID);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{uop.Operator.Value}' on type '{operand.Type}'", uop.Span);
            return new SemanticFailedOperation(new[] { operand });
        }

        return new SemanticUnaryOperation(operand, opKind.Value, uop.Span);
    }

    private SemanticExpression BindCountingOperation(CountingOperation co)
    {
        var name = (SemanticName) BindName(co.Name);

        if (name is null)
            return new SemanticFailedExpression(co.Span);

        var opKind = SemanticCountingOperation.GetOperationKind(co.Operator.Kind, name.Type, co.ReturnAfter);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{co.Operator.Value}' on type '{name.Type}'", co.Span);
            return new SemanticFailedExpression(co.Span);
        }

        return new SemanticCountingOperation(name, opKind.Value, co.Span);
    }

    private SemanticExpression BindConversionExpression(ConversionExpression ce)
    {
        var expr   = BindExpression(ce.Expression);
        var dest   = BindTypeClause(ce.Destination);
        var cvKind = SemanticConversionExpression.GetConversionKind(expr.Type, dest);
    
        if (cvKind is null)
        {
            if (expr.Type.IsKnown)
                Diagnostics.Report.CannotConvert(expr.Type.ToString(), dest.ToString(), ce.Span);
            return new SemanticFailedExpression(ce.Span);
        }
    
        return new SemanticConversionExpression(expr, dest, cvKind.Value, ce.Span);
    }

    private SemanticExpression BindBinaryOperation(BinaryOperation biop)
    {
        var left   = BindExpression(biop.LHS);
        var right  = BindExpression(biop.RHS);
        var opKind = SemanticBinaryOperation.GetOperationKind(biop.Operator.Kind, left.Type, right.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{biop.Operator.Value}' on types: '{left.Type}' and '{right.Type}'", biop.Span);
            return new SemanticFailedOperation(new[] { left, right });
        }

        var @operator = SemanticBinaryOperator.GetSemanticOperator(left.Type, opKind.Value, right.Type);

        return new SemanticBinaryOperation(left, @operator, right);
    }

    private SemanticTernaryOperation BindTernaryOperation(TernaryOperation terop)
    {
        var condition = BindExpression(terop.Condition, TypeSymbol.Boolean);
        var trueExpr  = BindExpression(terop.TrueExpression);
        var falseExpr = BindExpression(terop.FalseExpression);

        if (trueExpr.Type != falseExpr.Type)
            Except($"Types '{trueExpr.Type}' and '{falseExpr.Type}' don't match in ternary operation", terop.Span);

        return new(condition, trueExpr, falseExpr);
    }

    private SemanticExpression BindAssignExpression(AssignmentExpression aexpr)
    {
        var expr = aexpr.Operator is null
                 ? BindExpression(aexpr.Expression)
                 : BindBinaryOperation(new(aexpr.Assignee, aexpr.Operator, aexpr.Expression));

        var name = BindName(aexpr.Assignee);

        if (name is SemanticName var)
        {
            if (var.Symbol.IsConstant)
            {
                Diagnostics.Report.CannotAssignToConst(var.Symbol.Name, aexpr.Assignee.Span);
                return BindName(aexpr.Assignee);
            }

            if ((var.Symbol.Type, expr.Type).IsAssignable())
                Scope.Assign(var.Symbol);
            else
            {
                if (expr.Type.IsKnown)
                    Diagnostics.Report.TypesDoNotMatch(var.Symbol.Type.ToString(), expr.Type.ToString(), aexpr.Span);
                return BindName(aexpr.Assignee);
            }

            return new SemanticAssignment(var, expr, aexpr.Operator?.Value, aexpr.Span);
        }

        return expr;
    }
}
