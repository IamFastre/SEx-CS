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

    AssignmentExpression,
    CompoundAssignmentExpression,

    ParenExpression,
    UnaryOperation,
    BinaryOperation,
}