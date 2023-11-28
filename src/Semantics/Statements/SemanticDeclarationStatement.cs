using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public NameLiteral         Name       { get; }
    public Token?              Type       { get; }
    public ValType             TypeHint   { get; }
    public bool                IsConstant { get; }
    public SemanticExpression? Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(DeclarationStatement declaration, Token? type, SemanticExpression? expression)
    {
        Name       = declaration.Name;
        IsConstant = declaration.IsConstant;
        Type       = type;
        TypeHint   = GetNameType(type?.Value);
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
        CONSTS.RANGE   => ValType.Range,

        CONSTS.NUMBER  => ValType.Number,
        CONSTS.ANY     => ValType.Any,

        _ => ValType.Null,
    };
}
