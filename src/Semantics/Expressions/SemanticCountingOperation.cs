using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping.Symbols;

namespace SEx.Semantics;

internal enum CountingKind
{
    IncrementAfter,
    IncrementBefore,

    DecrementAfter,
    DecrementBefore,
}

internal class SemanticCountingOperation : SemanticExpression
{
    public SemanticVariable Name          { get; }
    public CountingKind     OperationKind { get; }

    public override Span         Span     { get; }
    public override TypeSymbol   Type => Name.Type;
    public override SemanticKind Kind => SemanticKind.CountingOperation;

    public SemanticCountingOperation(SemanticVariable name, CountingKind kind, Span span)
    {
        Name          = name;
        OperationKind = kind;

        Span          = span;
    }

    public static CountingKind? GetOperationKind(TokenKind op, TypeSymbol operand, bool returnAfter) => (op, operand.ID) switch
    {
        (TokenKind.Increment, TypeID.Integer or TypeID.Float or TypeID.Char)
            => returnAfter ? CountingKind.IncrementAfter : CountingKind.IncrementBefore,

        (TokenKind.Decrement, TypeID.Integer or TypeID.Float or TypeID.Char)
            => returnAfter ? CountingKind.DecrementAfter : CountingKind.DecrementBefore,

        _ => null,
    };

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Name;
    }
}