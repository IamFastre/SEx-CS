namespace SEx.Semantics;

public enum SemanticKind
{
    Statement,

    Literal,
    Name,

    AssignExpression,
    UnaryOperation,
    BinaryOperation,
    ParenExpression,

    FailedExpression,
}
