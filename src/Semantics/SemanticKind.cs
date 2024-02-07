namespace SEx.SemanticAnalysis;

public enum SemanticKind
{
    // Statements & Their clauses
    ProgramStatement,
    ExpressionStatement,
    BlockStatement,
    DeclarationStatement,
    DeletionStatement,
    FunctionStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ForStatement,
    ReturnStatement,

    // Literals
    Literal,
    FormatString,
    Range,
    List,
    Name,
    Function,

    // Expressions
    FailedExpression,
    AssignExpression,
    IndexAssignExpression,
    CallExpression,
    IndexingExpression,
    FailedOperation,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
    ConversionOperation,

    // Others
    StringFragment,
}
