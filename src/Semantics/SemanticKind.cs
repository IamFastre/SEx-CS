namespace SEx.Semantics;

public enum SemanticKind
{
    ProgramStatement,
    BlockStatement,
    DeclarationStatement,
    ExpressionStatement,
    IfStatement,
    ElseClause,

    Literal,
    Name,

    AssignExpression,
    ParenExpression,
    UnaryOperation,
    BinaryOperation,
    TernaryOperation,

    FailedExpression,
}
