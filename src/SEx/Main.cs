using System.Text;
using SEx.Diagnose;
using SEx.AST;
using SEx.Evaluation;
using SEx.Lex;
using SEx.Parse;
using SEx.Generic.Logic;
using SEx.Generic.Constants;
using SEx.Generic;

namespace SEx;

public class Runner
{
    public Runner(string source, string name)
    {
        Source      = source;
        Name        = name;
        Diagnostics = new();
        Value       = NodeValue.Unknown;
    }

    public string      Source      { get; }
    public string      Name        { get; }
    public Diagnostics Diagnostics { get; }
    public Token[]   ? Tokens      { get; protected set; }
    public SyntaxTree? SyntaxTree  { get; protected set; }
    public NodeValue   Value       { get; protected set; }

    public string[] Lines => Source.Split('\n');

    public void Start()
    {
        var lexer     = new Lexer(Source, Diagnostics);
        Tokens        = lexer.Tokens.ToArray();

        var parser    = new Parser(lexer);
        SyntaxTree    = parser.Tree;

        try
        {
            var evaluator = new Evaluation.Evaluator(parser);
            Value         = evaluator.Value;
        }
        catch (Exception e) { Console.WriteLine(e); }
    }

    public void Throw()
    {
        int start, end;
        string line;
        foreach (var er in Diagnostics.Exceptions)
        {
            (start, end) = (er.Span.Start.Index, er.Span.End.Index + 1);
            line = Source[start..end];
            er.Print(Name, line);
        }
    }
}

public class REPL
{

    private bool TreeShown    = false;
    private bool TokensShown  = false;
    private bool UseLiteral   = true;
    private bool PrintOut     = true;

    public void Start(string readlineChevron = "{#} ", string outputChevron = "")
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding  = Encoding.Unicode;
        }

        Runner runner;
        Console.Write(readlineChevron);
        string line = Console.ReadLine() ?? "";

        if (line == "\u0018")
            Environment.Exit(0);

        PrintOut = !CheckCommand(line);
        if (!PrintOut)
            line = "";

        runner = new(line, "<stdin>");
        runner.Start();

        runner.Throw();
        if (runner.Diagnostics.Exceptions.Count > 0)
            Console.WriteLine();

        string output;
        output = outputChevron + PrintValue(runner.Value);
        output = UseLiteral ? output.ToLiteral() : output;

        if (TokensShown)
        {
            Console.WriteLine($"• {C.RED}Tokens{C.END} ↴");
            foreach (var tk in runner.Tokens!)
                Console.Write($"{tk} ");
            Console.WriteLine('\n');
        }

        if (TreeShown)
        {
            Console.WriteLine($"• {C.RED}Tree{C.END} ↴ "); //↓
            runner.SyntaxTree?.Root?.PrintTree();
            Console.WriteLine();
        }

        if (PrintOut)
            Console.WriteLine(output);
    }

    private bool CheckCommand(string line)
    {
        bool match(string toWhat, bool remove = false)
        {
            var result = line.Trim().StartsWith(toWhat);
            if (result && remove)
                line = line.Trim().Replace(toWhat, "");

            return result;
        }

        if (!match("#", true))
            return false;

        if (match("TREE"))
        {
            TreeShown = !TreeShown;
            Console.WriteLine($"AST set to {TreeShown}");
        }

        if (match("TOKENS"))
        {
            TokensShown = !TokensShown;
            Console.WriteLine($"Tokens view set to {TokensShown}");
        }

        if (match("DEBUG"))
        {
            TreeShown   = !TreeShown;
            TokensShown = !TokensShown;
            UseLiteral  = !UseLiteral;
            Console.WriteLine($"AST set to {TreeShown}");
            Console.WriteLine($"Tokens view set to {TokensShown}");
            Console.WriteLine($"Literal view set to {UseLiteral}");
        }

        if (match("ESCAPE"))
        {
            UseLiteral = !UseLiteral;
            Console.WriteLine($"Literal view set to {UseLiteral}");
        }

        if (match("CLEAR"))
            Console.Clear();

        if (match("EXIT"))
            Environment.Exit(0);

        return true;
    }

    public static string PrintValue(NodeValue value)
    {
        switch (value.Kind)
        {
            case NodeKind.Unknown:
                return $"Null";

            case NodeKind.Boolean:
                return value.Value ? CONSTS.TRUE : CONSTS.FALSE;

            case NodeKind.Integer:
            case NodeKind.Float:
                return value.Value.ToString();

            case NodeKind.Char:
                return $"'{value.Value}'";

            case NodeKind.String:
                return $"\"{value.Value}\"";

            default:
               return "";
        }
    }
}