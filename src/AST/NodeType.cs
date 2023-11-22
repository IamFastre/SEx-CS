namespace SEx.AST;

public enum NodeKind
{
    Bad,
    Unknown,

    Statement,

    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String,
    Name,

    ExpressionStatement,
    DeclarationStatement,
    BlockStatement,

    AssignmentExpression,
    CompoundAssignmentExpression,

    ParenExpression,
    UnaryOperation,
    BinaryOperation,
}