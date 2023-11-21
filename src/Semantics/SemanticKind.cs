namespace SEx.Semantics;

public enum SemanticKind
{
    Statement,
    ExpressionStatement,
    BlockStatement,

    Literal,
    Name,

    AssignExpression,
    UnaryOperation,
    BinaryOperation,
    ParenExpression,

    FailedExpression,
}
