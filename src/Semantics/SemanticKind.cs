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
    ParenExpression,
    CallExpression,
    IndexingExpression,
    FailedExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
    FailedOperation,
    ConversionExpression,


    StringFragment,
}
