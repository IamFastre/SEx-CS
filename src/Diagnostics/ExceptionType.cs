namespace SEx.Diagnose;

public enum ExceptionType
{
    BaseException,
    SyntaxError,
    TypeError,
    MathError,
    StringParseError,
    OverflowError,
    InternalError,
    ParsingException,
    SymbolError,
    IndexError,
}

public enum Sender
{
    Unknown,
    Lexer,
    Parser,
    Analyzer,
    Scope,
    Evaluator
}
