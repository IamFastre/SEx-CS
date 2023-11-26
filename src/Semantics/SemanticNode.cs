using SEx.AST;
using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Semantics;

internal abstract class SemanticNode
{
    public abstract SemanticKind Kind { get; }
    public abstract Span Span { get; }

    public static ValType ToValueKind(NodeKind kind) => kind switch
    {
        NodeKind.Unknown => ValType.Unknown,
        NodeKind.Null    => ValType.Null,
        NodeKind.Boolean => ValType.Boolean,
        NodeKind.Integer => ValType.Integer,
        NodeKind.Float   => ValType.Float,
        NodeKind.Char    => ValType.Char,
        NodeKind.String  => ValType.String,

        _ => throw new Exception("Unknown literal kind"),
    };
}

internal abstract class SemanticStatement  : SemanticNode {}
internal abstract class SemanticClause     : SemanticNode {}
internal abstract class SemanticExpression : SemanticNode
{
    public abstract ValType Type { get; }
}
