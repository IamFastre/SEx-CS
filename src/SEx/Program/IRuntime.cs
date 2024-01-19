using SEx.AST;
using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping;
using SEx.Semantics;

namespace SEx.Main;

internal interface IRuntime
{
    public abstract Source                     Source            { get; }
    public abstract Diagnostics                Diagnostics       { get; }
    public abstract Scope                      Scope             { get; }
    public abstract SemanticScope              SemanticScope     { get; }
    public abstract Token[]?                   Tokens            { get; protected set; }
    public abstract ProgramStatement?          SimpleTree        { get; protected set; }
    public abstract SemanticProgramStatement?  SemanticTree      { get; protected set; }
    public abstract LiteralValue               Value             { get; protected set; }

    public    abstract LiteralValue Run();
    protected abstract void         ParseArguments(string[] arg);
}
