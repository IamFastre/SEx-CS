using SEx.Evaluation.Values;
using SEx.Generic.Text;
using SEx.Lexing;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public enum CountingKind
{
    IncrementAfter,
    IncrementBefore,

    DecrementAfter,
    DecrementBefore,
}

public class SemanticCountingOperation : SemanticExpression
{
    public SemanticName Name          { get; }
    public CountingKind OperationKind { get; }

    public override Span         Span     { get; }
    public override SemanticKind Kind => SemanticKind.CountingOperation;

    public SemanticCountingOperation(SemanticName name, CountingKind kind, Span span)
        : base(name.Type)
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