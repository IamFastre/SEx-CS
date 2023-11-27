namespace SEx.AST;

public enum NodeKind
{
    // Others
    Bad,
    Unknown,
    Token,

    // Literals
    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String,
    Range,
    Name,

    // Statements & Their clauses
    ProgramStatement,
    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,

    // Expressions
    AssignmentExpression,
    CompoundAssignmentExpression,
    ParenExpression,
    UnaryOperation,
    BinaryOperation,
    TernaryOperation,
}