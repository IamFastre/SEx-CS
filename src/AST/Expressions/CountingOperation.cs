using SEx.Lex;
using SEx.AST;
using SEx.Generic.Constants;
using SEx.Generic.Text;

namespace SEx.Parse;

internal class CountingOperation : Expression
{
    public bool        ReturnAfter { get; }
    public Token       Operator    { get; }
    public NameLiteral Name        { get; }

    public override Span     Span { get; }
    public override NodeKind Kind => NodeKind.CountingOperation;

    public CountingOperation(Token op, NameLiteral name, bool returnAfter)
    {
        Operator    = op;
        Name        = name;
        ReturnAfter = returnAfter;

        Span        = ReturnAfter
                    ? new(name.Span, op.Span)
                    : new(op.Span, name.Span);
    }

    public override string ToString()
        => ReturnAfter
         ? $"<{Name} {C.GREEN2}{Operator.Value}{C.END}>"
         : $"<{C.GREEN2}{Operator.Value}{C.END} {Name}>";

    public override IEnumerable<Node> GetChildren()
    {
        if (ReturnAfter)
        {
            yield return Name;
            yield return Operator.Node;
        }
        else
        {
            yield return Operator.Node;
            yield return Name;
        }
    }
}