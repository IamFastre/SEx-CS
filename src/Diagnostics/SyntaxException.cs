using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Generic.Logic;

namespace SEx.Diagnose;

public class SyntaxException
{
    public readonly ExceptionType Type;
    public readonly string        Text;
    public readonly Span          Span;
    public readonly ExceptionInfo Info;

    public SyntaxException(ExceptionType type, string text, Span span, ExceptionInfo? info = null)
    {
        Type = type;
        Text = text;
        Span = span;
        Info = info ?? ExceptionInfo.Default;
    }

    public override string ToString()
    {
        var preposition = Span.Start == Span.End ? "at" : "between";
        return $"{Type}: {Text}, {preposition} {Span}";
    }

    public void Print(string name = "<unknown>", string? line = null)
    {
        if (line is not null)
            if (C.UNDERLINE.Length > 0)
                AutoUnderline();
            else
                ManualUnderline();

        Console.WriteLine($"{C.RED}{Type}{C.END}: {C.RED2}{Text}{C.END}");

        if (line is not null)
            Console.WriteLine($"  {C.RED}×> {C.RED2}{C.DIM}{C.ITALIC}{line}{C.END}");
        Console.WriteLine($"  {C.YELLOW2}at {C.YELLOW}{name}{C.YELLOW2}, <{Span}>{C.END}");

        void AutoUnderline()
        {
            line = line.Insert(Span.Start.Column - 1, C.UNDERLINE)
                       .Insert(Span.End.Column + C.UNDERLINE.Length, C.ENDULINE);
        }

        void ManualUnderline()
        {
            char[] _ = " ".Repeat(line.Length).ToArray();

            for (int i = Span.Start.Column - 1; i < Span.End.Column; i++)
                _[i] = '^';
            
            line = line + "\n" + " ".Repeat(5) + new string(_);
        }
    }
}

public class ExceptionInfo
{
    public required Sender Sender;
    public bool ReReadLine = false;

    public static readonly ExceptionInfo Default = new() { Sender = Sender.Unknown };

    public static readonly ExceptionInfo Lexer     = new() { Sender = Sender.Lexer };
    public static readonly ExceptionInfo ReParser  = new() { Sender = Sender.Parser, ReReadLine = true };
    public static readonly ExceptionInfo Parser    = new() { Sender = Sender.Parser };
    public static readonly ExceptionInfo Analyzer  = new() { Sender = Sender.Analyzer };
    public static readonly ExceptionInfo Scope     = new() { Sender = Sender.Scope };
    public static readonly ExceptionInfo Evaluator = new() { Sender = Sender.Evaluator };
}