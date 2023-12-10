using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Diagnose;

public class Diagnostics
{
    public List<SyntaxException> Exceptions { get; }
    public Report                Report     { get; }

    public Diagnostics()
    {
        Exceptions = new();
        Report     = new(this);
    }

    public void Flush()
        => Exceptions.Clear();

    public void Add(ExceptionType type, string text, Span span, bool rereadLine = false)
        => Exceptions.Add(new SyntaxException(type, text, span, rereadLine));

    public void Throw(Source source)
    {
        foreach (var error in Exceptions)
            error.Print(source.Name, source.Lines[error.Span.Start.Line - 1]);
    }
}

public class Report
{
    private Diagnostics Diagnostics { get; }

    public Report(Diagnostics diagnostics)
    {
        Diagnostics = diagnostics;
    }

    private void Except(ExceptionType type, string message, Span span, bool rereadLine = false)
        => Diagnostics.Add(type, message, span, rereadLine);


    internal void ValuelessConstant(string name, Span span)
        => Except(ExceptionType.SyntaxError, $"No value was given to constant '{name}'", span);

    internal void UselessTypeAdded(string type, Span span)
        => Except(ExceptionType.SyntaxError, $"No need for added type '{type}'", span);

    internal void ExpectedType(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Expected {("aeiou".Contains(type1[0]) ? "an" : "a")} '{type1}' got {("aeiou".Contains(type2[0]) ? "an" : "a")} '{type2}'", span);

    internal void CannotConvert(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Cannot convert from '{type1}' to '{type2}'", span);

    internal void TypesDoNotMatch(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Types '{type1}' and '{type2}' do not match", span);

    internal void HeteroList(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Typed list can't have '{type1}' and '{type2}'", span);

    internal void CannotIterate(string type, Span span)
        => Except(ExceptionType.TypeError, $"Type '{type}' is not iterable", span);

    internal void CannotIndex(string type, Span span)
        => Except(ExceptionType.IndexError, $"Cannot perform indexing on '{type}'", span);

    internal void CannotIndexWithType(string type1, string type2, Span span)
        => Except(ExceptionType.IndexError, $"Cannot perform indexing on '{type1}' with type '{type2}'", span);

    internal void IndexOutOfBoundary(Span span)
        => Except(ExceptionType.IndexError, $"Index is out of boundary", span);

    internal void CannotAssignToConst(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Cannot assign to constant '{name}'", span);

    internal void UndefinedVariable(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is not defined", span);

    internal void AlreadyDefined(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is already defined", span);

    internal void AlreadyConstant(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is already a constant", span);

    internal void SymbolWrongUsage(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Wrong usage for symbol '{name}'", span);

    internal void InvalidTypeClause(Span span)
        => Except(ExceptionType.SymbolError, $"Type is invalid", span);
}
