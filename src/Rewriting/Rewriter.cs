using SEx.SemanticAnalysis;

namespace SEx.Rewriting;

public abstract class Rewriter
{
    public SemanticProgramStatement Rewrite(SemanticProgramStatement program)
        => new(program.Body.Select(RewriteStatement).ToArray());

    /* ====================================================================== */
    /*                               Statements                               */
    /* ====================================================================== */
    public SemanticStatement RewriteStatement(SemanticStatement stmt) => stmt switch
    {
        SemanticBlockStatement       blkStmt   => RewriteBlockStatement(blkStmt),
        SemanticExpressionStatement  exprStmt  => RewriteExpressionStatement(exprStmt),
        SemanticDeclarationStatement decStmt   => RewriteDeclarationStatement(decStmt),
        SemanticIfStatement          ifStmt    => RewriteIfStatement(ifStmt),
        SemanticWhileStatement       whileStmt => RewriteWhileStatement(whileStmt),
        SemanticForStatement         forStmt   => RewriteForStatement(forStmt),
        SemanticFunctionStatement    funcStmt  => RewriteFunctionStatement(funcStmt),
        SemanticReturnStatement      retStmt   => RewriteReturnStatement(retStmt),

        _ => throw new Exception("Statement unadded")
    };

    public abstract SemanticBlockStatement       RewriteBlockStatement(SemanticBlockStatement blkStmt);
    public abstract SemanticExpressionStatement  RewriteExpressionStatement(SemanticExpressionStatement exprStmt);
    public abstract SemanticDeclarationStatement RewriteDeclarationStatement(SemanticDeclarationStatement decStmt);
    public abstract SemanticIfStatement          RewriteIfStatement(SemanticIfStatement ifStmt);
    public abstract SemanticWhileStatement       RewriteWhileStatement(SemanticWhileStatement whileStmt);
    public abstract SemanticForStatement         RewriteForStatement(SemanticForStatement forStmt);
    public abstract SemanticFunctionStatement    RewriteFunctionStatement(SemanticFunctionStatement funcStmt);
    public abstract SemanticReturnStatement      RewriteReturnStatement(SemanticReturnStatement retStmt);


    /* ====================================================================== */
    /*                               Expressions                              */
    /* ====================================================================== */
    public SemanticExpression RewriteExpression(SemanticExpression expr) => expr switch
    {
        SemanticAssignment          asExpr   => RewriteAssignment(asExpr),
        SemanticIndexAssignment     inAsExpr => RewriteIndexAssignment(inAsExpr), // "in ass expression" lol
        SemanticCallExpression      callExpr => RewriteCallExpression(callExpr),
        SemanticIndexingExpression  inExpr   => RewriteIndexingExpression(inExpr),
        SemanticFailedExpression    failExpr => RewriteFailedExpression(failExpr),
        SemanticUnaryOperation      uOp      => RewriteUnaryOperation(uOp),
        SemanticCountingOperation   cOp      => RewriteCountingOperation(cOp),
        SemanticBinaryOperation     biOp     => RewriteBinaryOperation(biOp),
        SemanticTernaryOperation    terOp    => RewriteTernaryOperation(terOp),
        SemanticFailedOperation     failOp   => RewriteFailedOperation(failOp),
        SemanticConversionOperation conOp    => RewriteConversionOperation(conOp),

        _ => throw new Exception("Expression unadded")
    };

    public abstract SemanticAssignment          RewriteAssignment(SemanticAssignment asExpr);
    public abstract SemanticIndexAssignment     RewriteIndexAssignment(SemanticIndexAssignment inAsExpr);
    public abstract SemanticCallExpression      RewriteCallExpression(SemanticCallExpression callExpr);
    public abstract SemanticIndexingExpression  RewriteIndexingExpression(SemanticIndexingExpression inExpr);
    public abstract SemanticFailedExpression    RewriteFailedExpression(SemanticFailedExpression failExpr);
    public abstract SemanticUnaryOperation      RewriteUnaryOperation(SemanticUnaryOperation uOp);
    public abstract SemanticCountingOperation   RewriteCountingOperation(SemanticCountingOperation cOp);
    public abstract SemanticBinaryOperation     RewriteBinaryOperation(SemanticBinaryOperation biOp);
    public abstract SemanticTernaryOperation    RewriteTernaryOperation(SemanticTernaryOperation terOp);
    public abstract SemanticFailedOperation     RewriteFailedOperation(SemanticFailedOperation failOp);
    public abstract SemanticConversionOperation RewriteConversionOperation(SemanticConversionOperation conOp);
}
