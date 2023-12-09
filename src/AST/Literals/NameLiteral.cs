using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

internal sealed class NameLiteral : Expression
{
    public Token  Token           { get; }
    public string Value           { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.Name;

    public NameLiteral(Token token)
    {
        Token = token;
        Value = token.Value;
        Span  = Token.Span;
    }

    public sealed override IEnumerable<Node> GetChildren()
    {
        yield return Token.Node;
    }
}