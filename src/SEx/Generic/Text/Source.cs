
namespace SEx.Generic.Text;

public class Source
{
    public string   Name   { get; protected set; }
    public string   Text   { get; protected set; }

    public string[] Lines     => Text.Split('\n');
    public int      Length    => Text.Length;
    public int      LineCount => Lines.Length;

    public Source(string name, string[] lines)
        : this(name, string.Join(Environment.NewLine, lines)) { }

    public Source(string name, string text)
    {
        Name = name;
        Text  = text.Replace("\r", "");
    }

    public char   this[int      index] => Text[index];
    public char   this[Position pos  ] => Text[pos.Index + 1];
    public string this[Range    range] => Text[range];
    public string this[Span     span ] => Text[span.Start.Index..(span.End.Index + 1)];


    public int GetIndex(Position position)
    {
        int index = 0;

        for (int i = 0; i < position.Line - 1; i++)
            index += Lines[i].Length + 1;

        return index + position.Column - 1;
    }

    public Position GetLastPosition() => GetPosition(Text.Length - 1);

    public Position GetPosition(int index)
    {
        if  (index >= Text.Length)
            throw new IndexOutOfRangeException($"{index}");

        int line = 1;
        int column = 1;

        for (int i = 0; i < index; i++)            
            if (Text[i] == '\n')
            {
                line++;
                column = 1;
            }
            else
                column++;

        return new Position(line, column, index);
    }

    public void Append(string text)     => Text += text;
    public void AppendLine(string line) => Text += "\n" + line;

    public override string ToString() => Text;
    public string ToString(int start, int length) => Text.Substring(start, length);
    public string ToString(Span span) => this[span];
}