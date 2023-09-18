using SEx.Generic;

namespace SEx.Analysis;

public enum ExceptionType
{
    BaseException,
    SyntaxError,
    TypeError,
    StringParseError,
    OverflowError,
}

public class Diagnostics
{
    private List<SyntaxException> _exceptions;
    public List<SyntaxException> Exceptions => _exceptions;

    public Diagnostics()
    {
        _exceptions = new();
    }

    public void Add(ExceptionType type, string text, Span span) =>
        Exceptions.Add(new SyntaxException(type, text, span));
}

public class SyntaxException
{
    public readonly ExceptionType Type;
    public readonly string Text;
    public readonly Span Span;

    public SyntaxException(ExceptionType type, string text, Span span)
    {
        Type = type;
        Text = text;
        Span = span;
    }

    public override string ToString()
    {
        var preposition = Span.Start == Span.End ? "at" : "between";
        return $"{Type}: {Text} {preposition} {Span}";
    }

    // public void Print(string Name = "<unknown>", string? Line = null)
    // {
    //     if (Line is not null)
    //         Console.WriteLine($"{C.RED}×> {C.DIM}{C.ITALIC}{Line}{C.END}");

    //     Console.WriteLine(
    //         $"{C.RED}{Type}{C.END}: {C.RED2}{Text}{C.END}\n"
    //       + $"  {C.ORANGE}at {Name}, ({Span}){C.END}"
    //     );
    // }
}
