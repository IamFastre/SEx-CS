namespace SEx.Generic.Text;

public sealed class Position
{
    public int Line   { get; }
    public int Column { get; }
    public int Index  { get; }

    public Position(int line = 1, int column = 1, int index = 0)
    {
        Line   = line;
        Column = column;
        Index  = index;
    }

    public override bool Equals(object? obj) => obj is Position pos
                                             && Line   == pos.Line
                                             && Column == pos.Column
                                             && Index  == pos.Index;

    public override int GetHashCode() => HashCode.Combine(Line, Column, Index);

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }
}
