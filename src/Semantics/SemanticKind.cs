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
    ReturnStatement,

    // Literals
    Literal,
    FormatString,
    Range,
    List,
    Name,

    // Expressions
    AssignExpression,
    IndexAssignment,
    ParenExpression,
    Function,
    CallExpression,
    IndexingExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
    ConversionExpression,

    FailedOperation,
    FailedExpression,
    FunctionBodyStatement,
    StringFragment,
}
