using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Diagnosing;

public class SyntaxException
{
    public readonly ExceptionType Type;
    public readonly string        Text;
    public readonly bool          ReReadLine;
    public readonly Span          Span;

    public bool IsInternal => Span.Start.Index == -1;

    public SyntaxException(ExceptionType type, string text, Span span, bool rereadLine = false)
    {
        Type       = type;
        Text       = text;
        Span       = span;
        ReReadLine = rereadLine;
    }

    public override string ToString()
    {
        var preposition = Span.Start == Span.End ? "at" : "between";
        return $"{Type}: {Text}, {preposition} {Span}";
    }

    public void Print(string name = "<unknown>", string? line = null)
    {
        name = name.StartsWith("<") ? name : $"\"{name}\"";
        if (line is not null)
        {
            if (C.UNDERLINE.Length > 0)
                AutoUnderline();
            else
                ManualUnderline();
        }

        Console.WriteLine($"• {C.RED}{Type}{C.END}: {C.RED2}{Text}{C.END}");

        if (line is not null)
            Console.WriteLine($"    {C.RED}×> {C.RED2}{C.DIM}{C.ITALIC}{line}{C.END}");
            Console.WriteLine($"    {C.YELLOW2}at {C.YELLOW}{name}{C.YELLOW2}, <{(IsInternal ? "stdin" : Span)}>{C.END}");

        void AutoUnderline()
        {
            try
            {
                line = line.Insert(Span.Start.Column - 1, C.UNDERLINE)
                           .Insert(Span.End.Column + C.UNDERLINE.Length, C.ENDULINE);
            }
            catch {}
        }

        void ManualUnderline()
        {
            try
            {
                char[] _ = " ".Repeat(line.Length).ToArray();

                for (int i = Span.Start.Column - 1; i < Span.End.Column; i++)
                    _[i] = '^';

                line = line + "\n" + " ".Repeat(5) + new string(_);
            }
            catch {}
        }
    }
}
