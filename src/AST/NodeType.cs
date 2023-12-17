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
    CallExpression,
    IndexingExpression,
    UnaryOperation,
    CountingOperation,
    ConversionExpression,
    BinaryOperation,
    TernaryOperation,

    // Others
    SeparatedClause,
}