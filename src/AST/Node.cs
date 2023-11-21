using SEx.Generic.Text;
using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

public abstract class Node
{
    public Span     Span { get; protected set; }
    public NodeKind Kind { get; protected set; }

    public abstract override string ToString();
    public abstract IEnumerable<Node> GetChildren();

    public Node()
    {
        Kind = NodeKind.Bad;
        Span = new();
    }

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


        var children = GetChildren();
        if (this is Literal LT)
        {
            writer.WriteLine(leaf(LT.Kind.ToString(), LT.Value ?? ""));
            return;
        }

        writer.WriteLine($"[{C.BLUE2}{C.BLINK}{Kind}{C.END}]");
        foreach (var child in children)
        {
            if (child == children.LastOrDefault())
                writer.Write(indent + last);
            else
                writer.Write(indent + middle);


            if (child is Token TK)
            {
                writer.WriteLine(leaf(TK.Kind.ToString(), TK.Value ?? ""));
                continue;
            }

            if (child == children.LastOrDefault())
                child.WriteTree(writer, indent + space);
            else
                child.WriteTree(writer, indent + vertical);
        }
    }
}

public sealed class Statement : Node
{
    public Expression[] Body { get; private set; }

    public Statement(Expression[] expressions)
    {
        Body = expressions;
        Span = expressions.Length > 0
             ? new(Body.First().Span, Body.Last().Span)
             : new();
    }

    public override string ToString() => $"<Statement[{Body.Length}]>";
    public override IEnumerable<Node> GetChildren() => Body;
}

public abstract class Expression : Node {}