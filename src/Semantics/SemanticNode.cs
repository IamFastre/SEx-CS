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

internal sealed class SemanticStatement : SemanticNode
{
    public SemanticExpression[]  Body { get; private set; }
    public override Span         Span { get; }
    public override SemanticKind Kind => SemanticKind.Statement;

    public SemanticStatement(SemanticExpression[] expressions)
    {
        Body = expressions;
        Span = new(Body.First().Span, Body.Last().Span);
    }
}

internal abstract class SemanticExpression : SemanticNode
{
    public abstract ValType Type { get; }
}
