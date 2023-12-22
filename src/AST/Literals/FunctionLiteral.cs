using SEx.Lex;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parse;

public class FunctionLiteral : Expression
{
    public Token                            OpenParen  { get; }
    public SeparatedClause<ParameterClause> Parameters { get; }
    public Token                            CloseParen { get; }
    public TypeClause?                      Hint       { get; }
    public Token                            Arrow      { get; }
    public Statement                        Body       { get; }

    public override Span     Span                      { get; }
    public override NodeKind Kind => NodeKind.FunctionLiteral;

    public FunctionLiteral(Token openParen, SeparatedClause<ParameterClause> parameters, Token closeParen, TypeClause? hint, Token arrow, Statement statement)
    {
        OpenParen  = openParen;
        Parameters = parameters;
        CloseParen = closeParen;
        Hint       = hint;
        Arrow      = arrow;
        Body       = statement;

        Span       = new(openParen.Span, statement.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return OpenParen.Node;
        yield return Parameters;
        yield return CloseParen.Node;
        yield return Arrow.Node;
        yield return Body;
    }
}