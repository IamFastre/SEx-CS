using SEx.AST;

namespace SEx.Evaluation;

public class NodeValue
{
    public NodeValue(dynamic value, Node node) 
    {
        Value = value;
        Node  = node;
    }

    public dynamic Value { get; }
    public Node Node { get; }
    public NodeKind Kind => Value switch
    {
        null              => NodeKind.Unknown,
        true    or false  => NodeKind.Boolean,
        Int128  or int    => NodeKind.Integer,
        decimal or double => NodeKind.Float,
        char              => NodeKind.Char,
        string            => NodeKind.String,

        _ => throw new Exception("Unknown value")
    };

    public override string ToString()
    {
        var @return = Value is not null ? $"{Value}" : NodeKind.Unknown.ToString();
        return @return;
    }

    public static readonly NodeValue Unknown = new(null!, Literal.Unknown);
}
