namespace SEx.Semantics;

public enum SemanticKind
{
    // Statements & Their clauses
    ProgramStatement,
    BlockStatement,
    DeclarationStatement,
    ExpressionStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,
    ForStatement,

    // Literals
    Literal,
    Range,
    Variable,
    List,

    // Expressions
    AssignExpression,
    ParenExpression,
    IndexingExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,

    FailedOperation,
    FailedExpression,
}
