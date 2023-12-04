using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping;

namespace SEx.Semantics;

internal sealed class SemanticDeclarationStatement : SemanticStatement
{
    public VariableSymbol        Variable   { get; }
    public Span                  VarSpan    { get; }
    public DeclarationStatement  Node       { get; }
    public SemanticExpression?   Expression { get; }

    public override Span Span { get; }
    public override SemanticKind Kind => SemanticKind.DeclarationStatement;

    public SemanticDeclarationStatement(VariableSymbol var, Span span, DeclarationStatement node, SemanticExpression? expr)
    {
        Variable   = var;
        VarSpan    = span;
        Node       = node;
        Expression = expr;
        Span       = node.Span;
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

        _              => ValType.Any,
    };
}
