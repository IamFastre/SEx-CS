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
    List,

    // Statements & Their clauses
    ProgramStatement,
    ExpressionStatement,
    DeclarationStatement,
    TypeClause,
    GenericTypeClause,
    BlockStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,
    ForStatement,

    // Expressions
    AssignmentExpression,
    ParenthesizedExpression,
    IndexingExpression,
    UnaryOperation,
    CountingOperation,
    BinaryOperation,
    TernaryOperation,
}