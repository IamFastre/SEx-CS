namespace SEx.Generic;

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
