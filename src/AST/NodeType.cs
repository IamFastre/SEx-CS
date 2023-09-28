namespace SEx.AST;

public enum NodeKind
{
    Bad,
    Unknown,
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