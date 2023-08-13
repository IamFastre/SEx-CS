using Util;

namespace Diagnosing;

public enum ErrorType
{
    BaseException,
    SyntaxError,
}

public enum WarningType
{
    Test
}

public enum InfoType
{
    Test
}


public class Diagnostics
{

    public string Name;
    public string[] Lines;

    public List<Error> Errors { get => _errors; set => _errors = value; }
    public List<Warning> Warnings { get => _warnings; set => _warnings = value; }
    public List<Info> Infos { get => _infos; set => _infos = value; }

    public Diagnostics(string name, string source)
    {
        Name   = name;
        Lines = source.Replace("\r","").Split('\n');

        _errors   = new();
        _warnings = new();
        _infos    = new();
    }

    private List<Error> _errors;
    private List<Warning> _warnings;
    private List<Info> _infos;

    public static Diagnostics File(string path)
    {
        var name = Path.GetFileName(path);
        var source = System.IO.File.ReadAllText(path);

        return new Diagnostics(name, source);
    }

    public static Diagnostics REPL(string source)
    {
        var name = "<stdin>";

        return new Diagnostics(name, source);
    }

    public void Throw()
    {
        foreach (var err in _errors)
        {
            err.Print(Name, Lines[err.Span.Start.Line - 1]);
        }

        foreach (var wrn in _warnings)
        {
            wrn.Print(Name, Lines[wrn.Span.Start.Line - 1]);
        }

        foreach (var inf in _infos)
        {
            inf.Print(Name, Lines[inf.Span.Start.Line - 1]);
        }
    }

    public void NewError(ErrorType type, string text, Span span)   => Errors.Add(new Error(type, text, span));
    public void NewWarning(WarningType type, string text, Span span) => Warnings.Add(new Warning(type, text, span));
    public void NewInfo(InfoType type, string text, Span span)    => Infos.Add(new Info(type, text, span));
}

public class Error
{
    public readonly ErrorType Type;
    public readonly string Text;
    public readonly Span Span;

    public Error(ErrorType type, string text, Span span)
    {
        Type = type;
        Text = text;
        Span = span;
    }

    public void Print(string Name = "<unknown>", string? Line = null)
    {
        if (Line is not null)
            Console.WriteLine($"{C.RED}×> {C.DIM}{C.ITALIC}{Line}{C.END}");

        Console.WriteLine(
            $"{C.RED}{Type}{C.END}: {C.RED2}{Text}{C.END}\n"
          + $"  {C.ORANGE}at {Name}, ({Span}){C.END}");
    }
}

public class Warning
{
    public readonly WarningType Type;
    public readonly string Text;
    public readonly Span Span;

    public Warning(WarningType type, string text, Span span)
    {
        Type = type;
        Text = text;
        Span = span;
    }

    public void Print(string Name = "<unknown>", string? Line = null)
    {
        if (Line is not null)
            Console.WriteLine($"{C.YELLOW}#> {C.DIM}{C.ITALIC}{Line}{C.END}");

        Console.WriteLine(
            $"{C.ORANGE}{Type}{C.END}: {C.YELLOW}{Text}{C.END}\n"
          + $"  {C.YELLOW2}at {Name}, ({Span}){C.END}");
    }
}

public class Info
{
    public readonly InfoType Type;
    public readonly string Text;
    public readonly Span Span;

    public Info(InfoType type, string text, Span span)
    {
        Type = type;
        Text = text;
        Span = span;
    }

    public void Print(string Name = "<unknown>", string? Line = null)
    {
        if (Line is not null)
            Console.WriteLine($"{C.SKY}•> {C.DIM}{C.ITALIC}{Line}{C.END}");

        Console.WriteLine(
            $"{C.ITALIC}{C.BLUE}{Type}: {Text}{C.END}\n"
          + $"  at {Name}, ({Span})");
    }
}