using SEx.Generic.Constants;
using SEx.Lex;

namespace SEx.AST;

internal sealed class DeclarationStatement : Statement
{
    public DeclarationStatement(Token hash, Name name, Token? type = null, Expression? expression = null, bool isConst = false)
    {
        Hash       = hash;
        Name       = name;
        Type       = type;
        Expression = expression;
        IsConstant = isConst;

        Kind = NodeKind.DeclarationStatement;
        Span = new(hash.Span, expression?.Span ?? type?.Span ?? name.Span);
    }

    public Token       Hash       { get; }
    public Name        Name       { get; }
    public Token?      Type       { get; }
    public Expression? Expression { get; }
    public bool        IsConstant { get; }


    public override IEnumerable<Node> GetChildren()
    {
        yield return Hash;
        yield return Name;
        if (Expression != null)
            yield return Expression;
    }

    public override string ToString()
        => $"<{C.BLUE2}Declaration: {C.GREEN2}{Name} {C.BLUE2}=>{C.END} {Expression?.ToString() ?? CONSTS.NULL}{C.END}>";
}
