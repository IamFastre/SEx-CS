using System.Text;
using SEx.Diagnosing;
using SEx.AST;
using SEx.Evaluation;
using SEx.Lexing;
using SEx.Parsing;
using SEx.Generic.Logic;
using SEx.SemanticAnalysis;
using SEx.Evaluation.Values;
using SEx.Scoping;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;
using System.Diagnostics;

namespace SEx.Main.REPL;

internal sealed class REPL : IRuntime
{
    private bool DebugShown     = false;
    private bool TokensShown    = false;
    private bool TreeShown      = false;
    private bool ProgramShown   = false;
    private bool TreeLogged     = false;
    private bool TypeShown      = false;
    private bool UnescapedShown = false;
    private bool TimeShown      = false;
    private bool IsMonochrome   = false;

    private string ValueString => UnescapedShown ? Value.ToString().Unescape() : Value.ToString();
    private string Text        => Script.ToString();

    public static string Name      => "<stdin>";
    public static string PInputChv => SEx.IsValentine ? $"{SEx.PINK }<♥> {C.END}" : $"{C.GREEN2}<+> {C.END}";
    public static string SInputChv => SEx.IsValentine ? $"{C.MAGENTA}... {C.END}" : $"{C.BLUE2 }... {C.END}";
    public readonly string[] Args;

    public const char Prefix = '$';

    public REPL(string[] args)
    {
        Args = args;

        Line          = "";
        Script        = new();
        Diagnostics   = new(Source);
        SemanticScope = new();
        Scope         = new();
        Value         = UnknownValue.Template;

        ParseArguments(args);
    }

    public Diagnostics                Diagnostics       { get; }
    public Scope                      Scope             { get; }
    public SemanticScope              SemanticScope     { get; }
    public StringBuilder              Script            { get; }
    public Source                     Source            => new(Name, Text);
    public Stopwatch?                 Watch             { get; set; }
    public string                     Line              { get; set; }
    public Token[]?                   Tokens            { get; set; }
    public ProgramStatement?          SimpleTree        { get; set; }
    public SemanticProgramStatement?  SemanticTree      { get; set; }
    public LiteralValue               Value             { get; set; }
    public Exception?                 PreviousException { get; set; }

    public void ParseArguments(string[] args)
    {
        bool IsArg(string arg) => args.Contains(arg);
        bool AreArgs(params string[] _args) => _args.Any(IsArg);

        if (AreArgs("--monochrome", "-mch"))
            C.ToMono();

        if (AreArgs("--debug", "-d"))
            ToggleAll();

        if (AreArgs("--show-time", "-t"))
            ToggleTime();

        if (AreArgs("--show-tokens", "-tk"))
            ToggleTokens();

        if (AreArgs("--show-AST", "-ast"))
            ToggleTree();

        if (AreArgs("--show-program", "-prgm"))
            ToggleProgram();

        if (AreArgs("--show-escaped", "-esc"))
            ToggleEsc();
    }

    public static string[] commands =
    [
        "DEBUG", "CLEAR"  ,  "TOKENS" ,
        "TREE" , "PROGRAM",  "LOGTREE",
        "TYPE" , "ESCAPED",  "COLOR"  ,
        "TIME" , "EXIT"   ,  "RESET"  ,
                 "WELCOME",
    ];

    public void Throw()
    {
        Diagnostics.Throw();
        Diagnostics.Flush();
    }

    public void PrintValue()
        => Console.WriteLine(TypeShown ? $"<{C.YELLOW2}{Value.Type}{C.END}>: {ValueString}" : ValueString);

    public static void Run(string[] args)
    {
        Welcome();

        var repl = new REPL(args);
        repl.LoopWrapper();
    }

    private void LoopWrapper()
    {
        try
        {
            Loop();
        }
        catch (Exception e)
        {
            if (e.ToString() == PreviousException?.ToString())
                Console.WriteLine($"{C.RED}Dude... {C.RED2}stop making the same mistake.{C.END}");
            else
                Console.WriteLine($"{C.RED}Tsk tsk tks, {C.RED2}you did an oopsie.{C.END}");

            PreviousException = e;
            Console.WriteLine($"{C.BLINK}{C.BLACK2}Press enter to get the error...{C.END}");

            Console.Write(C.INVISIBLE);
            Console.ReadLine();
            Console.Write(C.END);

            Console.WriteLine($"{C.ITALIC}{C.DIM}{e}{C.END}\n");

            Console.Write($"{C.BLUE2}Continue? (y/N): {C.END}");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Reset();
                LoopWrapper();
            }
        }
    }

    private void Loop()
    {
        bool NewInput() => Script.Length == 0;
        while (true)
        {
            Console.Write(NewInput() ? PInputChv : SInputChv);
            Line = Console.ReadLine() ?? "";

            if (Line == "\u0018")
                Environment.Exit(0);

            else if (NewInput() && string.IsNullOrWhiteSpace(Line))
                continue;

            else if (IsCommand())
                ParseCommand();

            else
            {
                Script.Append(Line);
                Diagnostics.Source = Source;

                var lexer = new Lexer(Source, Diagnostics);
                Tokens = lexer.Lex();

                var parser = new Parser(Tokens, Diagnostics);
                SimpleTree = parser.Parse();

                if (Diagnostics.Exceptions.Any((SyntaxException e) => e.ReReadLine)
                && !string.IsNullOrWhiteSpace(Line))
                {
                    Diagnostics.Flush();
                    Script.Append('\n');
                    continue;
                }

                Watch?.Restart();
                var analyzer = new Analyzer(SimpleTree, SemanticScope, Diagnostics);
                SemanticTree = analyzer.Analyze();

                if (!Diagnostics.Exceptions.Any())
                {
                    var evaluator = new Evaluator(SemanticTree, Scope, Diagnostics);
                    Value = evaluator.Evaluate();
                }
                else
                    Value = UnknownValue.Template;
                Watch?.Stop();

                PrintDebugs();
                Throw();

                if (!(Value.Type == TypeSymbol.Void))
                    PrintValue();

                Reset();
            }
        }
    }

    private static void Welcome()
    {
        Console.WriteLine($"{C.BLUE2}SEx-{CONSTS._VERSION_} ({C.YELLOW2}{Environment.UserName} {C.BLUE2}on {C.RED2}{Environment.OSVersion.Platform}{C.BLUE2}){C.END}");
        Console.WriteLine($"{C.BLUE2}{C.DIM}{C.ITALIC}Type: {C.GREEN2}'{Prefix}HELP' {C.BLUE2}for more info.{C.END}");
    }

    private void Reset()
    {
        Script.Clear();
        Diagnostics.Flush();
        Value = UnknownValue.Template;
    }

    private void PrintDebugs()
    {
        if (TokensShown)
        {
            Console.WriteLine($"• {C.GREEN2}Tokens{C.END} ↴");
            foreach (var tk in Tokens!)
                Console.Write(tk.GetString());
            Console.WriteLine('\n');
        }

        if (TreeShown)
        {
            Console.WriteLine($"• {C.GREEN2}Tree{C.END} ↴"); //↓
            SimpleTree?.PrintTree();
            Console.WriteLine();
        }

        if (ProgramShown)
        {
            Console.WriteLine($"• {C.GREEN2}Program{C.END} ↴"); //↓
            SemanticTree?.PrintTree();
            Console.WriteLine();
        }

        if (TreeLogged)
        {
            SimpleTree?.LogTree(Line);
            Console.WriteLine();
        }

        if (TimeShown)
            Console.WriteLine($"• {C.GREEN2}Time Elapsed{C.END}: {C.RED2}{Watch!.Elapsed.TotalMicroseconds/1000f:0.############}{C.YELLOW2}ms{C.END}");
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

            case "PROGRAM":
                ToggleProgram();
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

            case "TIME":
                ToggleTime();
                break;

            case "DEBUG":
                ToggleAll();
                break;

            case "WELCOME":
                Welcome();
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
        ToggleProgram(DebugShown);
        ToggleType(DebugShown);
        ToggleEsc(DebugShown);
    }

    private void ToggleTime(bool _ = false)
    {
        TimeShown = !TimeShown || _;
        Watch     = TimeShown ? new Stopwatch() : null;
        Console.WriteLine($"Show time set to: {TimeShown}");
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

    private void ToggleProgram(bool _ = false)
    {
        ProgramShown = !ProgramShown || _;
        Console.WriteLine($"Show Program set to: {ProgramShown}");
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
        Console.WriteLine($"Show {C.RED2}c{C.YELLOW}o{C.YELLOW2}l{C.GREEN}o{C.BLUE}r{C.MAGENTA}s{C.END} set to: {!IsMonochrome}");
    }
}
