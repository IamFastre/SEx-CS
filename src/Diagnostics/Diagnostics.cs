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
        => Diagnostics.Add(type, message, span);


    internal void ValuelessConstant(string name, Span span)
        => Except(ExceptionType.SyntaxError, $"No value was given to constant '{name}'", span);

    internal void UselessTypeAdded(string type, Span span)
        => Except(ExceptionType.SyntaxError, $"No need for added type '{type}'", span);

    internal void TypesDoNotMatch(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Types '{type1}' and '{type2}' do not match", span);

    internal void CannotIterate(string type, Span span)
        => Except(ExceptionType.TypeError, $"Cannot iterate type '{type}'", span);

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
}
