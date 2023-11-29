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

    // Literals
    Literal,
    Range,
    Name,

    // Expressions
    AssignExpression,
    ParenExpression,
    IndexingExpression,
    UnaryOperation,
    BinaryOperation,
    TernaryOperation,

    FailedExpression,
}
