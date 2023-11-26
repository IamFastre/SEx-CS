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
    UnaryOperation,
    BinaryOperation,
    ParenExpression,

    FailedExpression,
}
