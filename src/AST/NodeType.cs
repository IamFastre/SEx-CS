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
    // Conditional
    IfStatement,
    WhileStatement,
    ElseClause,
    ForStatement,
    ReturnStatement,

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