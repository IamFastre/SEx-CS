using SEx.Generic.Text;

namespace SEx.Semantics;

internal sealed class SemanticProgramStatement : SemanticStatement
{
    public SemanticStatement[] Body   { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.ProgramStatement;

    public SemanticProgramStatement(SemanticStatement[] statements)
    {
        Body       = statements;
        Span = Body.Length > 0 ? new(Body.First().Span, Body.Last().Span) : new();
    }

    public static readonly SemanticProgramStatement Empty = new(Array.Empty<SemanticStatement>()); 
}