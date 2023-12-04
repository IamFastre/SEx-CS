using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Lex;
using SEx.Scoping;

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

    public override Span         Span   { get; }
    public override ValType      Type => Name.Type;
    public override SemanticKind Kind => SemanticKind.CountingOperation;

    public SemanticCountingOperation(SemanticVariable name, CountingKind kind, Span span)
    {
        Name          = name;
        OperationKind = kind;

        Span          = span;
    }

    public static CountingKind? GetOperationKind(TokenKind op, ValType operand, bool returnAfter) => (op, operand) switch
    {
        (TokenKind.Increment, ValType.Integer or ValType.Float)
            => returnAfter ? CountingKind.IncrementAfter : CountingKind.IncrementBefore,

        (TokenKind.Decrement, ValType.Integer or ValType.Float)
            => returnAfter ? CountingKind.DecrementAfter : CountingKind.DecrementBefore,

        _ => null,
    };
}