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
    Name,

    // Statements
    ProgramStatement,
    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,
    IfStatement,
    ElseClause,

    // Expressions
    AssignmentExpression,
    CompoundAssignmentExpression,
    ParenExpression,
    UnaryOperation,
    BinaryOperation,
    TernaryOperation,
}