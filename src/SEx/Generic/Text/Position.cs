namespace SEx.Generic.Text;

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

     public override string ToString()
     {
        return $"{Line}:{Column}";
     }
}
