using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class ForStatement : Statement
{
    public Token       For        { get; }
    public NameLiteral Variable   { get; }
    public Expression  Iterable   { get; }
    public Statement   Body       { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ForStatement;

    public ForStatement(Token @for, NameLiteral name, Expression iterable, Statement body)
    {
        For      = @for;
        Variable = name;
        Iterable = iterable;
        Body     = body;

        Span     = new(@for.Span, body.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return For.Node;
        yield return Variable;
        yield return Iterable;
        yield return Body;
    }
}