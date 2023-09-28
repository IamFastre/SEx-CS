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
