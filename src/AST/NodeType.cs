namespace SEx.AST;

public enum NodeKind
{
    // Others
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
    FunctionStatement,
    ParameterClause,
    ReturnStatement,
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,
    ForStatement,
    BreakStatement,
    ContinueStatement,

    // Expressions
    AssignmentExpression,
    ParenthesizedExpression,
    FunctionLiteral,
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