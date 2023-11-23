namespace SEx.Semantics;

public enum SemanticKind
{
    Statement,
    ProgramStatement,
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
