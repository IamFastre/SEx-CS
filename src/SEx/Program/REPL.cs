using System.Text;
using SEx.Diagnose;
using SEx.AST;
using SEx.Evaluate;
using SEx.Lex;
using SEx.Parse;
using SEx.Generic.Logic;
using SEx.Semantics;
using SEx.Evaluate.Values;
using SEx.Namespaces;
using SEx.Generic.Constants;

namespace SEx.Program;

internal class REPL
{
    private bool DebugShown     = false;
    private bool TokensShown    = false;
    private bool TreeShown      = false;
    private bool TreeLogged     = false;
    private bool TreeLineShown  = false;
    private bool TypeShown      = false;
    private bool UnescapedShown = false;
    private bool IsMonochrome   = false;

    private string ValueString => UnescapedShown ? Value.ToString().Unescape() : Value.ToString();
    private string Text        => Script.ToString();

    public static string Name      => "<stdin>";
    public static string PInputChv => $"{C.GREEN2}<+> {C.END}";
    public static string SInputChv => $"{C.BLUE2 }... {C.END}";
    public readonly string[] Args;

    public const char Prefix = '.';

    public REPL(string[] args)
    {
        Args = args;

        ParseArguments();

        Line         = "";
        Script       = new();
        Diagnostics  = new();
        Scope        = new(Diagnostics);
        Value        = UnknownValue.Template;

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding  = Encoding.Unicode;
        }
    }

    public Diagnostics         Diagnostics  { get; }
    public Scope               Scope        { get; }
    public StringBuilder       Script       { get; }
    public string              Line         { get; protected set; }
    public Token[]?            Tokens       { get; protected set; }
    public Statement?          SimpleTree   { get; protected set; }
    public SemanticStatement?  SemanticTree { get; protected set; }
    public LiteralValue        Value        { get; protected set; }

    private void ParseArguments()
    {
        bool IsArg(string arg) => Args.Contains(arg);
        bool AreArgs(params string[] args) => args.Any(IsArg);

        if (AreArgs("--monochrome", "-mch"))
            C.ToMono();
    }

    public static string[] commands =
    {
        "DEBUG", "CLEAR",    "TOKENS",
        "TREE",  "TREELINE", "LOGTREE",
        "TYPE",  "ESCAPED",  "COLOR",
        "EXIT",  "RESET",
    };

    public void Throw()
    {
        foreach (var er in Diagnostics.Exceptions)
            er.Print(Name, Text);

        Diagnostics.Flush();
    }

    public void PrintValue()
        => Console.WriteLine(TypeShown ? $"<{C.YELLOW2}{Value.Type.str()}{C.END}>: {ValueString}" : ValueString);

    public void Loop()
    {
        Console.WriteLine($"{C.BLUE2}SEx-{CONSTS._VERSION_} ({C.YELLOW2}{Environment.UserName} {C.BLUE2}on {C.RED2}{Environment.OSVersion.Platform}{C.BLUE2}){C.END}");
        Console.WriteLine($"{C.BLUE2}{C.DIM}{C.ITALIC}Type: {C.GREEN2}'help' {C.BLUE2}for more info.{C.END}");

        while (true)
        {
            Console.Write(Script.Length == 0 ? PInputChv : SInputChv);
            Line = Console.ReadLine() ?? "";

            if (Line == "\u0018")
                Environment.Exit(0);

            else if (string.IsNullOrWhiteSpace(Line))
                continue;

            else if (IsCommand())
                ParseCommand();

            else
            {
                Script.Append(Line);

                var lexer     = new Lexer(new(Text), Diagnostics);
                Tokens        = lexer.Lex();

                var parser    = new Parser(lexer);
                SimpleTree    = parser.Parse();

                if (Diagnostics.Exceptions.RemoveAll((SyntaxException e) => e.Info.ReReadLine) > 0)
                    continue;

                PrintDebugs();

                var analyzer  = new Analyzer(parser.Tree!, Diagnostics, Scope);
                SemanticTree  = analyzer.Analyze();

                var evaluator = new Evaluator(analyzer);
                Value         = evaluator.Evaluate();

                Throw();

                if (!(Value.Type == ValType.Void)) 
                    PrintValue();

                Script.Clear();
            }
        }
    }

    private void PrintDebugs()
    {
        if (TokensShown)
        {
            Console.WriteLine($"• {C.GREEN2}Tokens{C.END} ↴");
            foreach (var tk in Tokens!)
                Console.Write($"{tk} ");
            Console.WriteLine('\n');
        }

        if (TreeShown)
        {
            Console.WriteLine($"• {C.GREEN2}Tree{C.END} ↴"); //↓
            SimpleTree?.PrintTree();
            Console.WriteLine();
        }

        if (TreeLineShown)
        {
            Console.WriteLine($"• {C.GREEN2}Tree Line{C.END} ↴"); //↓
            Console.WriteLine(SimpleTree!.ToString());
            Console.WriteLine();
        }

        if (TreeLogged)
        {
            SimpleTree?.LogTree(Line);
            Console.WriteLine();
        }
    }

    private bool IsCommand()
        => Line.Trim().Length > 1
        && Line.Trim()[0] == Prefix
        && commands.Contains(Line.Trim()[1..]);

    private void ParseCommand()
    {
        var command = Line.Trim()[1..].Trim();

        switch(command)
        {
            case "TREE":
                ToggleTree();
                break;

            case "TREELINE":
                ToggleTreeLine();
                break;

            case "LOGTREE":
                ToggleLogTree();
                break;

            case "TOKENS":
                ToggleTokens();
                break;

            case "TYPE":
                ToggleType();
                break;

            case "ESCAPED":
                ToggleEsc();
                break;

            case "DEBUG":
                ToggleAll();
                break;

            case "CLEAR":
                Console.Clear();
                break;

            case "COLOR":
                ToggleColor();
                break;

            case "RESET":
                Scope.Flush();
                Diagnostics.Flush();
                Script.Clear();
                Console.WriteLine("All flushed!");
                break;

            case "EXIT":
                Environment.Exit(0);
                break;
        }
    }


    private void ToggleAll(bool _ = false)
    {
        DebugShown = !DebugShown || _;

        ToggleTokens(DebugShown);
        ToggleTree(DebugShown);
        ToggleTreeLine(DebugShown);
        ToggleType(DebugShown);
        ToggleEsc(DebugShown);
    }

    private void ToggleEsc(bool _ = false)
    {
        UnescapedShown = !UnescapedShown || _;
        Console.WriteLine($"Show unescaped value set to: {UnescapedShown}");
    }

    private void ToggleType(bool _ = false)
    {
        TypeShown = !TypeShown || _;
        Console.WriteLine($"Show value type set to: {TypeShown}");
    }

    private void ToggleTokens(bool _ = false)
    {
        TokensShown = !TokensShown || _;
        Console.WriteLine($"Show tokens set to: {TokensShown}");
    }

    private void ToggleTree(bool _ = false)
    {
        TreeShown = !TreeShown || _;
        Console.WriteLine($"Show AST set to: {TreeShown}");
    }

    private void ToggleTreeLine(bool _ = false)
    {
        TreeLineShown = !TreeLineShown || _;
        Console.WriteLine($"Show ASTL set to: {TreeLineShown}");
    }

    private void ToggleLogTree(bool _ = false)
    {
        TreeLogged = !TreeLogged || _;
        Console.WriteLine($"Log AST set to: {TreeLogged}");
    }

    private void ToggleColor()
    {
        IsMonochrome = !IsMonochrome;
        if (IsMonochrome)
            C.ToMono();
        else
            C.ToPoly();
        Console.WriteLine($"Show {C.RED2}c{C.YELLOW}o{C.YELLOW2}l{C.GREEN}o{C.BLUE}r{C.VIOLET}s{C.END} set to: {!IsMonochrome}");
    }
}
