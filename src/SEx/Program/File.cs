using SEx.AST;
using SEx.Diagnosing;
using SEx.Evaluation;
using SEx.Evaluation.Values;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lexing;
using SEx.Parsing;
using SEx.Scoping;
using SEx.SemanticAnalysis;

namespace SEx.Main.Files;

internal sealed class SExFile : IRuntime
{
    public string                     FilePath          { get; }
    public Source                     Source            { get; }
    public Diagnostics                Diagnostics       { get; }
    public Scope                      Scope             { get; } = new();
    public SemanticScope              SemanticScope     { get; } = new();
    public Token[]?                   Tokens            { get; set; }
    public ProgramStatement?          SimpleTree        { get; set; }
    public SemanticProgramStatement?  SemanticTree      { get; set; }
    public LiteralValue               Value             { get; set; } = UnknownValue.Template;

    private SExFile(string[] args)
    {
        Source      = GetSource(args[0]);
        Diagnostics = new(Source);

        if (!Path.Exists(args[0]))
        {
            Diagnostics.Report.SourcePathNotFound(args[0]);
            FilePath = Source.Name;
        }
        else
            FilePath = args[0];

        ParseArguments(args[1..]);
    }

    public static Source GetSource(string path)
        => Path.Exists(path) ? new(Path.GetFileName(path), File.ReadAllText(path)) : Source.Empty;

    public static void Run(string[] args)
    {
        var file = new SExFile(args);
        file.RunFile();
    }

    public void RunFile()
    {
        try
        {
            var lexer     = new Lexer(Source, Diagnostics);
            Tokens        = lexer.Lex();

            var parser    = new Parser(Tokens, Diagnostics);
            SimpleTree    = parser.Parse();

            var analyzer  = new Analyzer(SimpleTree, SemanticScope, Diagnostics);
            SemanticTree  = analyzer.Analyze();

            var evaluator = new Evaluator(SemanticTree, Scope, Diagnostics);
            Value         = evaluator.Evaluate();
        }
        catch (Exception e)
        {
            Diagnostics.Report.InternalError(e.Source, e.Message);
            Value = UnknownValue.Template;
        }

        Throw();
    }

    public void Throw()
    {
        Diagnostics.Throw();
        Diagnostics.Flush();
    }

    public void ParseArguments(string[] args)
    {
        bool IsArg(string arg) => args.Contains(arg);
        bool AreArgs(params string[] _args) => _args.Any(IsArg);

        if (AreArgs("--monochrome", "-mch"))
            C.ToMono();
    }
}
