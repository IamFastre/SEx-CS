using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public abstract class Node
{
    public abstract Span     Span { get; }
    public abstract NodeKind Kind { get; }

    public abstract override string ToString();
    public abstract IEnumerable<Node> GetChildren();

    public void LogTree(string source = "")
    {
        var wasColored = C.IsPolychrome;
        C.ToMono();

        var filename = $"LOG-{DateTime.Now.ToLocalTime():yyMMddHHmmssffffff}.txt";

        SEx.Path.Prepare();
        using var log = new StreamWriter(Path.Combine(SEx.Path.Logs, filename));
        log.WriteLine($"{DateTime.Now.ToLocalTime():dddd, dd MMM yyy HH:mm:ss.ffffff}");
        log.WriteLine();
        log.WriteLine($">> Input:");
        log.WriteLine($"{source}{Environment.NewLine}");
        log.WriteLine();
        log.WriteLine($">> Tree:");
        WriteTree(log, "");

        if (wasColored)
            C.ToPoly();

        Console.WriteLine($"• {C.GREEN2}Log recorded to '{C.RED2}{Path.Combine(SEx.Path.Logs, filename)}{C.GREEN2}'.{C.END}");
    }

    public void PrintTree()
        => WriteTree(Console.Out, "");

    public void WriteTree(TextWriter writer, string indent = "")
    {
        string middle   = C.DIM + "├──" + C.END;
        string vertical = C.DIM + "│  " + C.END;
        string last     = C.DIM + "└──" + C.END;
        string space    = C.DIM + "   " + C.END;

        // string PINK = C.RGB(252, 88, 217);

        static string leaf(string a, string b)
            => $"<{C.YELLOW2}{a}: {C.GREEN2}'{b}'{C.END}>";


        var children = GetChildren().ToArray();
        if (this is Literal LT)
        {
            writer.WriteLine(leaf(LT.Kind.ToString(), LT.Value ?? ""));
            return;
        }

        writer.WriteLine($"[{C.BLUE2}{C.BLINK}{Kind}{C.END}]");

        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];

            if (child == children.LastOrDefault())
                writer.Write(indent + last);
            else
                writer.Write(indent + middle);


            if (child is TokenNode TN)
            {
                writer.WriteLine(leaf(TN.Token.Kind.ToString(), TN.Value ?? ""));
                continue;
            }

            if (i == children.Length - 1)
                child.WriteTree(writer, indent + space);
            else
                child.WriteTree(writer, indent + vertical);
        }
    }
}

internal abstract class Statement  : Node {}
internal abstract class Clause     : Node {}
internal abstract class Expression : Node {}
