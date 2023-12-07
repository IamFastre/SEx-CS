using SEx.AST;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal abstract class SemanticNode
{
    public abstract SemanticKind Kind { get; }
    public abstract Span Span { get; }

    public static TypeSymbol ToValueKind(NodeKind kind) => kind switch
    {
        NodeKind.Unknown => TypeSymbol.Unknown,
        NodeKind.Null    => TypeSymbol.Null,
        NodeKind.Boolean => TypeSymbol.Boolean,
        NodeKind.Integer => TypeSymbol.Integer,
        NodeKind.Float   => TypeSymbol.Float,
        NodeKind.Char    => TypeSymbol.Char,
        NodeKind.String  => TypeSymbol.String,

        _ => throw new Exception("Unknown literal kind"),
    };

    public abstract IEnumerable<SemanticNode> GetChildren();
}

internal abstract class SemanticStatement  : SemanticNode {}
internal abstract class SemanticClause     : SemanticNode {}
internal abstract class SemanticExpression : SemanticNode
{
    public abstract TypeSymbol Type { get; }
}
