namespace SEx.Semantics;

public enum SemanticKind
{
    // Statements & Their clauses
    ProgramStatement,
    BlockStatement,
    DeclarationStatement,
    ExpressionStatement,
    FunctionStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,
    ForStatement,

    // Literals
    Literal,
    Range,
    List,
    Name,

    // Expressions
    AssignExpression,
    ParenExpression,
    CallExpression,
    IndexingExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
    ConversionExpression,

    FailedOperation,
    FailedExpression,
}
