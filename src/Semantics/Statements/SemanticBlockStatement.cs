using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticBlockStatement : SemanticStatement
{
    public Token               OpenBrace  { get; }
    public SemanticStatement[] Body       { get; }
    public Token               CloseBrace { get; }

    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.BlockStatement;

    public SemanticBlockStatement(Token openBrace, SemanticStatement[] statements, Token closeBrace)
    {
        OpenBrace  = openBrace;
        Body       = statements;
        CloseBrace = closeBrace;

        Span = Body.Length > 0 ? new(Body.First().Span, Body.Last().Span) : new();
    }
}
