using SEx.Scoping.Symbols;
using SEx.Generic.Text;

namespace SEx.Semantics;

public class SemanticFunctionStatement : SemanticStatement
{
    public FunctionSymbol    Function   { get; }
    public TypeSymbol        Type       { get; }
    public ParameterSymbol[] Parameters { get; }
    public SemanticStatement Body       { get; }

    public override Span         Span   { get; }
    public override SemanticKind Kind => SemanticKind.FunctionStatement;

    public SemanticFunctionStatement(FunctionSymbol func, TypeSymbol type, ParameterSymbol[] parameters, SemanticStatement body, Span span)
    {
        Function   = func;
        Type       = type;
        Parameters = parameters;
        Body       = body;

        Span       = span;
    }

    public override IEnumerable<SemanticNode> GetChildren()
    {
        yield return Body;
    }
}