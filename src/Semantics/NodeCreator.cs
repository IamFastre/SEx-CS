using SEx.Generic.Text;
using SEx.Scoping.Symbols;
using SEx.SemanticAnalysis;

namespace SEx.Creating;

internal static class NodeCreator
{
    public static SemanticExpressionStatement State(SemanticExpression expr)
        => new(expr);

    public static SemanticBlockStatement Block(IEnumerable<SemanticStatement> stmts, Span span)
        => new(stmts, span);

    public static SemanticDeclarationStatement Declare(NameSymbol var, SemanticExpression expr, Span span)
        => new(var, expr, span);

    public static SemanticDeclarationStatement VarDeclare(string name, SemanticExpression expr, Span span)
        => new(new NameSymbol(name, expr.Type, false), expr, span);

    public static SemanticDeclarationStatement ConstDeclare(string name, SemanticExpression expr, Span span)
        => new(new NameSymbol(name, expr.Type, true), expr, span);

    public static SemanticDeletionStatement Delete(NameSymbol var, Span span)
        => new(new(var, span), span);

    public static SemanticDeletionStatement Delete(SemanticName var)
        => new(var, var.Span);

    public static SemanticWhileStatement While(SemanticExpression condition, SemanticStatement body, SemanticStatement? elseStmt, Span span)
        => new(condition, body, elseStmt, span);

    public static SemanticAssignment Assign(NameSymbol var, SemanticExpression expr, Span span)
        => new(Var(var, span), expr, null, span);

    public static SemanticName Var(NameSymbol var, Span span)
        => new(var, span);

    public static NameSymbol Symbol(string name, TypeSymbol? type = null, bool isConst = false)
        => new(name, type ?? TypeSymbol.Any, isConst);

    public static SemanticCountingOperation Increment(SemanticName name)
        => new(name, CountingKind.IncrementBefore, name.Span);

    public static SemanticBinaryOperation Binary(SemanticExpression left, SemanticBinaryOperator @operator, SemanticExpression right, Span span)
        => new(left, @operator, right, span);

    public static SemanticBinaryOperator BiOperator(TypeSymbol left, BinaryOperationKind opKind, TypeSymbol right)
        => SemanticBinaryOperator.GetSemanticOperator(left, opKind, right);

    public static SemanticIndexingExpression Indexing(SemanticExpression iterable, SemanticExpression index, TypeSymbol type, Span span)
        => new(iterable, index, type, span);

    public static SemanticCallExpression Length(SemanticExpression iterable)
        => new(Var(BuiltIn.LengthOf.GetSymbol(), iterable.Span), TypeSymbol.Integer, new[] { iterable }, iterable.Span);

    public static SemanticLiteral Literal(string literal, TypeSymbol type, Span span)
       => new(literal, type, span);
}
