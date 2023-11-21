namespace SEx.Diagnose;

public enum ExceptionType
{
    BaseException,
    SyntaxError,
    TypeError,
    StringParseError,
    OverflowError,
    InternalError,
    ParsingException,
    SymbolError,
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
