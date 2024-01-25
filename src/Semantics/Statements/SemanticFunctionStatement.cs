using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.SemanticAnalysis;

public class SemanticFunctionStatement : SemanticStatement
{
    public NameSymbol        Function   { get; }
    public TypeSymbol        ReturnType { get; }
    public NameSymbol[]      Parameters { get; }
    public SemanticStatement Body       { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.FunctionStatement;

    public SemanticFunctionStatement(NameSymbol func, NameSymbol[] parameters, TypeSymbol returnType, SemanticStatement body, Span span)
    {
        Function   = func;
        Parameters = parameters;
        ReturnType = returnType;
        Body       = body;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Body;
    }
}