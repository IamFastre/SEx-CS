using SEx.Generic;

namespace SEx.Diagnose;

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
        return $"{Type}: {Text}, {preposition} {Span}";
    }

    public void Print(string Name = "<unknown>", string? Line = null)
    {

        Console.WriteLine($"{C.RED}{Type}{C.END}: {C.RED2}{Text}{C.END}");
        if (Line is not null)
        Console.WriteLine($"  {C.RED}×> {C.DIM}{C.ITALIC}{Line}{C.END}");
        Console.WriteLine($"  {C.YELLOW2}at {Name}, <{Span}>{C.END}");
    }
}
