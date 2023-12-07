using SEx.AST;
using SEx.Diagnose;
using SEx.Scoping;
using SEx.Scoping.Symbols;
using SEx.Generic.Text;
using SEx.Parse;

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

    private SemanticIfStatement BindIfStatement(IfStatement @is)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(@is.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(@is.Then);

        if (@is.ElseClause is not null)
            elseClause = BindElseClause(@is.ElseClause);

        return new(@is.If, condition, thenStmt, elseClause);
    }

    private SemanticWhileStatement BindWhileStatement(WhileStatement ws)
    {
        SemanticElseClause? elseClause = null;

        var condition = BindExpression(ws.Condition, TypeSymbol.Boolean);
        var thenStmt  = BindStatement(ws.Body);

        if (ws.ElseClause is not null)
            elseClause = BindElseClause(ws.ElseClause);

        return new(ws.While, condition, thenStmt, elseClause);
    }

    private SemanticElseClause BindElseClause(ElseClause ec)
    {
        var elseStmt = BindStatement(ec.Body);
        return new(ec.Else, elseStmt);
    }

    private SemanticForStatement BindForStatement(ForStatement fs)
    {
        var iterable = BindExpression(fs.Iterable);
        var elemType = iterable.Type.ElementType;

        if (elemType is null)
            Diagnostics.Report.CannotIterate(iterable.Type.ToString(), iterable.Span);

        Scope = new(Scope);

        var variable = DeclareVariable(fs.Variable, elemType ?? TypeSymbol.Unknown, true);
        var body     = BindStatement(fs.Body);

        Scope = Scope.Parent!;

        return new(fs.For, variable, iterable, body);
    }

    private SemanticBlockStatement BindBlockStatement(BlockStatement bs)
    {
        List<SemanticStatement> statements = new();

        foreach (var statement in bs.Body)
            statements.Add(BindStatement(statement));

        return new(bs.OpenBrace, statements.ToArray(), bs.CloseBrace);
    }

    private SemanticDeclarationStatement BindDeclarationStatement(DeclarationStatement ds)
    {
        var expr = ds.Expression is not null ? BindExpression(ds.Expression) : null;
        var hint = ds.TypeClause is not null ? GetType(ds.TypeClause) : TypeSymbol.Any;
        var var  = new VariableSymbol(ds.Variable.Value, hint, ds.IsConstant);

        if (ds.IsConstant && expr is null)
        {
            if (Scope.TryResolve(var.Name, out var symbol))
            {
                if (ds.TypeClause is not null)
                    Diagnostics.Report.UselessTypeAdded(hint.Name, ds.TypeClause.Span);
                else if (((VariableSymbol) symbol!).IsConstant)
                    Diagnostics.Report.AlreadyConstant(ds.Variable.Value, ds.Variable.Span);
                else
                    Scope.Assign(var);
            }
            else
                Diagnostics.Report.ValuelessConstant(ds.Variable.Value, ds.Variable.Span);
        }

        else if (expr is not null && !(hint, expr.Type).IsAssignable())
            Diagnostics.Report.TypesDoNotMatch(hint.ToString(), expr.Type.ToString(), ds.Span);

        else if (!Scope.TryDeclare(var))
            Diagnostics.Report.AlreadyDefined(var.Name, ds.Variable.Span);

        return new(var, ds.Variable.Span, ds, expr);
    }

    private SemanticExpressionStatement BindExpressionStatement(ExpressionStatement es)
    {
        var expr = BindExpression(es.Expression);
        return new(expr);
    }

    //=====================================================================//
    
    private VariableSymbol DeclareVariable(NameLiteral n, TypeSymbol type, bool isConst = false)
    {
        var symbol = new VariableSymbol(n.Value, type, isConst);
        Scope.Symbols.Add(n.Value, symbol);
        return symbol;
    }
    private TypeSymbol[] GetTypes(TypeClause[] tcs)
    {
        List<TypeSymbol> types = new();
        foreach (var tc in tcs)
            types.Add(GetType(tc));
        
        return types.ToArray();
    }

    private TypeSymbol GetType(TypeClause tc)
    {
        var type = tc.Kind == NodeKind.TypeClause
                 ? TypeSymbol.GetTypeByString(tc.Type.Value)
                 : GenericTypeSymbol.GetTypeByString(tc.Type.Value, GetTypes(((GenericTypeClause) tc).Parameters));

        if (type is null)
        {
            Diagnostics.Report.InvalidTypeClause(tc.Span);
            return TypeSymbol.Unknown;
        }

        for (int i = 0; i < tc.ListDimension; i++)
            type = GenericTypeSymbol.TypedList(type);
        
        return type;
    }

    private VariableSymbol? GetVariable(NameLiteral n)
    {
        switch (Scope.Resolve(n.Value))
        {
            case VariableSymbol variable:
                return variable;

            case null:
                Diagnostics.Report.UndefinedVariable(n.Value, n.Span);
                return null;

            default:
                Diagnostics.Report.SymbolWrongUsage(n.Value, n.Span);
                return null;
        }
    }

    //=====================================================================//

    private SemanticExpression BindExpression(Expression expr, TypeSymbol expected)
    {
        var val = BindExpression(expr);
        if (!expected.Matches(val.Type))
        {
            if (!expected.IsKnown)
            Except($"Expected an expression of type '{expected}'", expr.Span);
        }
        
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

            case NodeKind.ParenthesizedExpression:
                return BindParenExpression((ParenthesizedExpression) expr);

            case NodeKind.IndexingExpression:
                return BindIndexingExpression((IndexingExpression) expr);

            case NodeKind.UnaryOperation:
                return BindUnaryOperation((UnaryOperation) expr);

            case NodeKind.CountingOperation:
                return BindCountingOperation((CountingOperation) expr);

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
        var symbol = GetVariable(n);
        if (symbol == null)
            return new SemanticFailedExpression(n.Span);

        return new SemanticVariable(symbol, n.Span);
    }

    private SemanticList BindList(ListLiteral ll)
    {
        if (ll.Elements.Length > 0)
        {
            List<SemanticExpression> expressions = new();
            var arRef = BindExpression(ll.Elements.First());
            expressions.Add(arRef);

            foreach (var elem in ll.Elements[1..])
            {
                var expr = BindExpression(elem);

                if (expr.Type == arRef.Type)
                    expressions.Add(expr);
                else
                    Except($"list of type '{arRef.Type}' can't have a '{expr.Type}' element", ll.Span);
            }

            return new(expressions.ToArray(), arRef.Type, ll.Span);
        }

        return new(Array.Empty<SemanticExpression>(), TypeSymbol.Any, ll.Span);
    }

    private SemanticParenExpression BindParenExpression(ParenthesizedExpression pe)
    {
        var expr = pe.Expression is null ? null : BindExpression(pe.Expression);
        return new(pe.OpenParen, expr, pe.CloseParen);
    }

    private SemanticExpression BindIndexingExpression(IndexingExpression ie)
    {
        var iterable    = BindExpression(ie.Iterable);
        var index       = BindExpression(ie.Index);
        var elementType = iterable.Type.ElementType;

        if (elementType is null)
        {
            Except($"Can't perform indexing on '{iterable.Type}'", ie.Iterable.Span);
            return new SemanticFailedExpressions(new[] { iterable, index });
        }

        return new SemanticIndexingExpression(iterable, index, elementType ?? TypeSymbol.Unknown, ie.Span);
    }

    private SemanticExpression BindUnaryOperation(UnaryOperation uop)
    {
        var operand = BindExpression(uop.Operand);
        var opKind  = SemanticUnaryOperation.GetOperationKind(uop.Operator.Kind, operand.Type.ID);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{uop.Operator.Value}' on type '{operand.Type}'", uop.Span);
            return new SemanticFailedExpressions(new[] { operand });
        }

        return new SemanticUnaryOperation(operand, opKind.Value, uop.Span);
    }

    private SemanticExpression BindCountingOperation(CountingOperation co)
    {
        var name = (SemanticVariable) BindName(co.Name);

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

    private SemanticExpression BindBinaryOperation(BinaryOperation biop)
    {
        var left   = BindExpression(biop.LHS);
        var right  = BindExpression(biop.RHS);
        var opKind = SemanticBinaryOperation.GetOperationKind(biop.Operator.Kind, left.Type, right.Type);

        if (opKind is null)
        {
            Except($"Cannot apply operator '{biop.Operator.Value}' on types: '{left.Type}' and '{right.Type}'", biop.Span);
            return new SemanticFailedExpressions(new[] { left, right });
        }

        return new SemanticBinaryOperation(left, opKind.Value, right);
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
        var expr = aexpr.Operation is null
                 ? BindExpression(aexpr.Expression)
                 : BindBinaryOperation(new(aexpr.Assignee, aexpr.Operation, aexpr.Expression));

        var name = GetVariable(aexpr.Assignee);

        if (name is null)
            return expr;

        if (name.IsConstant)
        {
            Diagnostics.Report.CannotAssignToConst(name.Name, aexpr.Assignee.Span);
            return BindName(aexpr.Assignee);
        }

        if ((name.Type, expr.Type).IsAssignable())
            Scope.Assign(name);
        else
        {
            Diagnostics.Report.TypesDoNotMatch(name.Type.ToString(), expr.Type.ToString(), aexpr.Span);
            return BindName(aexpr.Assignee);
        }
        
        return new SemanticAssignment(name, expr, aexpr.Span);
    }
}
