namespace SEx.Semantics;

public enum SemanticKind
{
    Statement,
    BlockStatement,
    DeclarationStatement,
    ExpressionStatement,

    Literal,
    Name,

    AssignExpression,
    UnaryOperation,
    BinaryOperation,
    ParenExpression,

    FailedExpression,
}
