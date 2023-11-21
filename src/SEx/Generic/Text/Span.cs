namespace SEx.Generic.Text;

public class Span
{
    public Position Start, End;
    public int Length => End.Index - Start.Index + 1;

    public Span()
    {
        Start = new Position();
        End   = new Position();
    }

    public Span(Position start, Position? end = null)
    {
        Start  = start ?? new Position();
        End    = end   ?? Start;
    }

    public Span(Span start, Span end)
    {
        Start = start.Start;
        End   = end.End;
    }

    public override string ToString()
        => Start.Index == End.Index ? $"{Start}" : $"{Start} => {End}";

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
