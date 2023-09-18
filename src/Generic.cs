namespace SEx.Generic;


public class Position
{
    public int Line;
    public int Column;
    public int Index;

    public Position(int line = 1, int column = 1, int index = 0)
    {
        Line   = line;
        Column = column;
        Index  = index;
    }

    public static readonly Position Template = new(0,0,0);

     public override string ToString()
     {
        return $"{Line}:{Column}";
     }
}

public class Span
{
    public Position Start, End;
    public int Length;

    public Span(Position start, Position? end = null)
    {
        Start = start;
        End = end ?? start;
        Length = End.Index - Start.Index + 1;
    }

    public static readonly Span Template = new(Position.Template);

    public override string ToString()
    {
        if (Start != End)
            return $"{Start} => {End}";
        else
            return Start.ToString();
    }

    public bool Includes(Span other)
    {
        if (End is null)
            throw new ArgumentException("The span doesn't include an end point.");

        return Start.Index <= other.Start.Index && End.Index >= other.Start.Index;
    }

    // I know it can be simpler but I really don't care
    public bool Intersects(Span other)
    {
        if (other.End is null)
            throw new ArgumentException("The span doesn't include an end point.");

        if (Start.Index >= other.Start.Index && Start.Index <= other.End.Index)
            return true;

        if (End is not null)
            if (End.Index >= other.End.Index && End.Index >= other.Start.Index)
                return true;

        return false;
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
    public static readonly string RED       = "\u001b[31m";
    public static readonly string RED2      = "\u001b[91m";
    public static readonly string ORANGE    = "\u001b[33m";
    public static readonly string YELLOW    = "\u001b[33m";
    public static readonly string YELLOW2   = "\u001b[93m";
    public static readonly string GREEN     = "\u001b[32m";
    public static readonly string LETTUCE   = "\u001b[92m";
    public static readonly string SKY       = "\u001b[94m";
    public static readonly string BLUE      = "\u001b[34m";
    public static readonly string VIOLET    = "\u001b[35m";

    public static string RGB(int R, int G, int B, bool isBackground = false)
    {
        var BG = isBackground ? 48 : 38;
        return $"\\33[{BG};2;{R};{G};{B}m";
    }
}