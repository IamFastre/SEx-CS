using SEx.Generic;
using SEx.Lex;

namespace SEx.AST;

public abstract class Node
{
    public Span? Span;
    public NodeKind Kind { get; protected set; }

    public abstract override string ToString();
    public abstract IEnumerable<Node> GetChildren();

    public void PrintTree(string indent = "")
    {
        string middle   = C.DIM + "├──" + C.END;
        string vertical = C.DIM + "│  " + C.END;
        string last     = C.DIM + "└──" + C.END;
        string space    = C.DIM + "   " + C.END;

        // string PINK = C.RGB(252, 88, 217);

        static string leaf(string a, string b)
            => $"<{C.YELLOW2}{a}: {C.LETTUCE}'{b}'{C.END}>";


        var children = GetChildren();
        if (this is Literal LT)
        {
            Console.WriteLine(leaf(LT.Kind.ToString(), LT.Value ?? ""));
            return;
        }

        Console.WriteLine($"[{C.SKY}{C.BLINK}{Kind}{C.END}]");
        foreach (var child in children)
        {
            if (child == children.LastOrDefault())
                Console.Write(indent + last);
            else
                Console.Write(indent + middle);


            if (child is Token TK)
            {
                Console.WriteLine(leaf(TK.Kind.ToString(), TK.Value ?? ""));
                continue;
            }

            if (child == children.LastOrDefault())
                child.PrintTree(indent + space);
            else
                child.PrintTree(indent + vertical);
        }
    }
}

public abstract class Statement : Node {}
public abstract class Expression : Statement {}