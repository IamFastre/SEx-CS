namespace SEx.AST;

public enum NodeKind
{
    Bad,
    Unknown,

    Null,
    Boolean,
    Integer,
    Float,
    Char,
    String,

    Identifier,
    ParenExpression,
    UnaryOperation,
    BinaryOperation,
}