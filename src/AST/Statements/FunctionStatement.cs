using SEx.Lexing;
using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parsing;

internal class FunctionStatement : Statement
{
    public Token                            Symbol     { get; }
    public bool                             IsConstant { get; }
    public NameLiteral                      Name       { get; }
    public TypeClause?                      Hint       { get; }
    public SeparatedClause<ParameterClause> Parameters { get; }
    public Statement                        Body       { get; }

    public override Span     Span                      { get; }
    public override NodeKind Kind => NodeKind.FunctionStatement;

    public FunctionStatement(Token symbol, bool isConst, NameLiteral name, TypeClause? typeHint, SeparatedClause<ParameterClause> parameters, Statement body)
    {
        Symbol     = symbol;
        IsConstant = isConst;
        Name       = name;
        Hint       = typeHint;
        Parameters = parameters;
        Body       = body;

        Span       = new(symbol.Span, body.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Symbol.Node;
        yield return Name;
        if (Hint is not null)
            yield return Hint;
        yield return Parameters;
        yield return Body;
    }
}