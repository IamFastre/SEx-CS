using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

public sealed class SemanticBlockStatement : SemanticStatement
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

        Span =new(openBrace.Span, closeBrace.Span);
    }

    public override IEnumerable<SemanticNode> GetChildren() => Body;
}
