namespace SEx.SemanticAnalysis;

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
