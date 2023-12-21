using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.AST;

public sealed class DeclarationStatement : Statement
{
    public Token       Hash       { get; }
    public NameLiteral Variable   { get; }
    public TypeClause? TypeClause { get; }
    public Expression  Expression { get; }
    public bool        IsConstant { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.DeclarationStatement;

    public DeclarationStatement(Token hash, bool isConst, NameLiteral name, Expression expression, TypeClause? type = null)
    {
        Hash       = hash;
        IsConstant = isConst;
        Variable   = name;
        TypeClause = type;
        Expression = expression;

        Span       = new(hash.Span, expression?.Span ?? type?.Span ?? name.Span);
    }


    public override IEnumerable<Node> GetChildren()
    {
        yield return Hash.Node;
        yield return Variable;
        if (TypeClause != null)
            yield return TypeClause;
        if (Expression != null)
            yield return Expression;
    }
}
