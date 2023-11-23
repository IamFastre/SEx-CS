using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public Name                Name       { get; }
    public Token?              Type       { get; }
    public ValType             NameType   { get; }
    public bool                IsConstant { get; }
    public SemanticExpression? Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(DeclarationStatement declaration, Token? type, SemanticExpression? expression)
    {
        Name       = declaration.Name;
        IsConstant = declaration.IsConstant;
        Type       = type;
        NameType   = GetNameType(type?.Value);
        Expression = expression;
        Span       = declaration.Span;
    }

    public static ValType GetNameType(string? type) => type switch
    {
        CONSTS.BOOLEAN => ValType.Boolean,
        CONSTS.INTEGER => ValType.Integer,
        CONSTS.FLOAT   => ValType.Float,
        CONSTS.CHAR    => ValType.Char,
        CONSTS.STRING  => ValType.String,

        _ => ValType.Null,
    };
}
