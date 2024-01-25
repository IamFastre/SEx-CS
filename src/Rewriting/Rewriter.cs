using SEx.SemanticAnalysis;

namespace SEx.Rewriting;

public abstract class Rewriter
{
    public SemanticProgramStatement Rewrite(SemanticProgramStatement program)
        => new(program.Body.Select(RewriteStatement).ToArray());


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
}