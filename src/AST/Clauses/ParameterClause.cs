using SEx.AST;
using SEx.Generic.Text;

namespace SEx.Parsing;

public class ParameterClause : Clause
{
    public NameLiteral Name { get; }
    public TypeClause  Type { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.ParameterClause;

    public ParameterClause(NameLiteral name, TypeClause type)
    {
        Name = name;
        Type = type;

        Span = new(name.Span, type.Span);
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Name;
        yield return Type;
    }
}