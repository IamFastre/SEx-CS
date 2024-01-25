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
    AssignExpression,
    IndexAssignExpression,
    CallExpression,
    IndexingExpression,
    FailedExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
    FailedOperation,
    ConversionOperation,


    StringFragment,
}
