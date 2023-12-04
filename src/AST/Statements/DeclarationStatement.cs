using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

internal sealed class DeclarationStatement : Statement
{
    public Token       Hash       { get; }
    public NameLiteral Variable       { get; }
    public Token?      Type       { get; }
    public Expression? Expression { get; }
    public bool        IsConstant { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.DeclarationStatement;

    public DeclarationStatement(Token hash, NameLiteral name, Token? type = null, Expression? expression = null, bool isConst = false)
    {
        Hash       = hash;
        Variable       = name;
        Type       = type;
        Expression = expression;
        IsConstant = isConst;

        Span       = new(hash.Span, expression?.Span ?? type?.Span ?? name.Span);
    }


    public override IEnumerable<Node> GetChildren()
    {
        yield return Hash.Node;
        yield return Variable;
        if (Expression != null)
            yield return Expression;
    }

    public override string ToString()
        => $"<{C.BLUE2}Declaration: {C.GREEN2}{Variable} {C.BLUE2}=>{C.END} {Expression?.ToString() ?? CONSTS.NULL}{C.END}>";
}
