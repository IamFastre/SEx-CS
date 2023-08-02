using System.Data;
using System.Text.Json;

namespace Util;

public static class JSONReader
{
    public static T? Read<T>(string filePath)
    {
        string text = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(text);
    }
}

public class Position
{
    public int Line;
    public int Column;

    public Position(int line = 1, int column = 1)
    {
        this.Line = line;
        this.Column = column;
    }

     public override string ToString()
     {
        return $"(Ln {Line}, Col {Column})";
     }
}

public class Span
{
    public Position  Start;
    public Position? End;

    public Span(Position start, Position? end = null)
    {
        this.Start = start;
        this.End = end;
    }

    public override string ToString()
    {
        if (End != null)
            return $"{Start}:{End}";
        else
            return Start.ToString();

    }
}

public class C
{
    public static readonly string END       = "\u001b[0m";
    public static readonly string BOLD      = "\u001b[1m";
    public static readonly string DIM       = "\u001b[2m";
    public static readonly string ITALIC    = "\u001b[3m";
    public static readonly string UNDERLINE = "\u001b[4m";
    public static readonly string BLINK     = "\u001b[5m";
    public static readonly string BLINK2    = "\u001b[6m";
    public static readonly string SELECTED  = "\u001b[7m";
    public static readonly string INVISIBLE = "\u001b[8m";
    public static readonly string STRIKE    = "\u001b[9m";

    public static readonly string WHITE     = "\u001b[37m";
    public static readonly string BLACK     = "\u001b[30m";
    public static readonly string GRAY      = "\u001b[33m";
    public static readonly string RED       = "\u001b[31m";
    public static readonly string RED2      = "\u001b[91m";
    public static readonly string ORANGE    = "\u001b[33m";
    public static readonly string GOLD      = "\u001b[93m";
    public static readonly string YELLOW    = "\u001b[93m";
    public static readonly string GREEN     = "\u001b[32m";
    public static readonly string LETTUCE   = "\u001b[92m";
    public static readonly string SKY       = "\u001b[96m";
    public static readonly string BLUE      = "\u001b[34m";
    public static readonly string VIOLET    = "\u001b[35m";

    public static string RGB(int R, int G, int B, bool isBackground = false)
    {
        var BG = isBackground ? 48 : 38;
        return $"\\33[{BG};2;{R};{G};{B}m";
    }
}