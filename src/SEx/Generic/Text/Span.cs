namespace SEx.Generic.Text;

public class Span
{
    public Position Start { get; set; }
    public Position End   { get; set; }

    public int Length => End.Index - Start.Index + 1;

    public Span()                     : this(new Position(), new Position()) { }
    public Span(Position start)       : this (start, start)                  { }
    public Span(Span start, Span end) : this (start.Start, end.End)          { }

    public Span(Position start, Position end)
    {
        Start = start;
        End   = end;
    }

    public override string ToString()
        => Start.Index == End.Index ? $"{Start}" : $"{Start} => {End}";
}
