using SEx.Generic.Text;

namespace SEx.Diagnose;

public class Diagnostics
{
    public Source                Source     { get; set; }
    public Report                Report     { get; }
    public List<SyntaxException> Exceptions => Report.Exceptions;

    public Diagnostics(Source source)
        => (Source, Report) = (source, new(source));

    public void Flush()
        => Exceptions.Clear();

    public void Add(ExceptionType type, string text, Span span, bool rereadLine = false)
        => Exceptions.Add(new SyntaxException(type, text, span, rereadLine));

    public SyntaxException[] Sorted()
        => Exceptions.OrderBy(e => e.Span.Start.Index).ToArray();

    public void Throw()
        => Array.ForEach(Sorted(), e => e.Print(Source.Name, e.IsInternal ? null : Source.Lines[e.Span.Start.Line - 1]));
}

public record Report(Source Source)
{
    public List<SyntaxException> Exceptions { get; } = new();

    private void Except(ExceptionType type, string message, Span span, bool rereadLine = false)
        => Exceptions.Add(new SyntaxException(type, message, span, rereadLine));

    internal void UnrecognizedChar(char chr, Span span)
        => Except(ExceptionType.SyntaxError, $"Unrecognized character '{chr}' (U+{(int)chr:X4})", span);

    internal void UnterminatedString(Span span)
        => Except(ExceptionType.SyntaxError, $"Unterminated string literal", span);

    internal void InvalidSyntax(string token, Span span)
        => Except(ExceptionType.SyntaxError, $"Invalid syntax '{token}'", span);

    internal void Expected(string token, Span span, bool rereadLine)
        => Except(ExceptionType.SymbolError, $"Expected {("euioa".Contains(token[0]) ? "an" : "a")} '{token}'", span, rereadLine);

    internal void ExpectedToken(string token, string got, Span span, bool rereadLine)
        => Except(ExceptionType.SymbolError, $"Expected {("euioa".Contains(token[0]) ? "an" : "a")} '{token}' got '{got}'", span, rereadLine);

    internal void StatementExpected(Span span)
        => Except(ExceptionType.SyntaxError, $"Expected a statement", span, span.End.Equals(Source.GetLastPosition()));

    internal void ExpressionExpected(Span span)
        => Except(ExceptionType.SyntaxError, $"Expected an expression", span, span.End.Equals(Source.GetLastPosition()));

    internal void ExpressionExpectedAfter(string after, Span span)
        => Except(ExceptionType.SyntaxError, $"Expected an expression after '{after}'", span, span.End.Equals(Source.GetLastPosition()));

    internal void ExpressionExpectedBefore(string before, Span span)
        => Except(ExceptionType.SyntaxError, $"Expected an expression before '{before}'", span, span.End.Equals(Source.GetLastPosition()));

    internal void NameExpected(Span span)
        => Except(ExceptionType.SyntaxError, $"Expected a name", span, span.End.Equals(Source.GetLastPosition()));

    internal void NameExpected(string after, Span span)
        => Except(ExceptionType.SyntaxError, $"Expected a name after '{after}'", span, span.End.Equals(Source.GetLastPosition()));

    internal void ValuelessConstant(string constant, Span span)
        => Except(ExceptionType.SyntaxError, $"No value was given to constant '{constant}'", span, span.End.Equals(Source.GetLastPosition()));

    internal void InvalidAssignee(Span span)
        => Except(ExceptionType.SyntaxError, $"Assignee is invalid", span);

    internal void UselessTypeAdded(Span span)
        => Except(ExceptionType.SyntaxError, $"No need for added type", span);

    internal void UnexpectedEOF(Span span)
        => Except(ExceptionType.SyntaxError, $"Didn't expect program to end yet", span, span.End.Equals(Source.GetLastPosition()));

    internal void OperandMustBeName(string op, Span span)
        => Except(ExceptionType.TypeError, $"Operand of '{op}' must be a name", span);

    internal void TypeExpected(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Expected {("aeiou".Contains(type1[0]) ? "an" : "a")} '{type1}' got {("aeiou".Contains(type2[0]) ? "an" : "a")} '{type2}'", span);

    internal void CannotConvert(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Cannot convert from '{type1}' to '{type2}'", span);

    internal void CannotAssignType(string type, Span span)
        => Except(ExceptionType.TypeError, $"Cannot assign type '{type}'", span);

    internal void TypesDoNotMatch(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Types '{type1}' and '{type2}' do not match", span);

    internal void HeteroList(string type1, string type2, Span span)
        => Except(ExceptionType.TypeError, $"Typed list can't have '{type1}' and '{type2}'", span);

    internal void CannotIterate(string type, Span span)
        => Except(ExceptionType.TypeError, $"Type '{type}' is not iterable", span);

    internal void NotCallable(string type, Span span)
        => Except(ExceptionType.TypeError, $"Type '{type}' is not callable", span);

    internal void InvalidArgumentCount(string function, int expected, int gotten, Span span)
        => Except(ExceptionType.TypeError, $"Function '{function}' takes {expected} arguments got {gotten}", span);

    internal void ReturnNotExpected(Span span)
        => Except(ExceptionType.TypeError, $"Cannot use 'return' outside of functions", span);

    internal void NoReturnValueExpected(Span span)
        => Except(ExceptionType.TypeError, $"Didn't expect a return value", span);

    internal void ReturnValueExpected(Span span)
        => Except(ExceptionType.TypeError, $"Expected a return value", span);

    internal void TypeNotMutable(string type, Span span)
        => Except(ExceptionType.TypeError, $"Type '{type}' is not mutable", span);

    internal void BadRangeDirection(Span span)
        => Except(ExceptionType.MathError, "Range end point and step direction don't match", span);

    internal void RangeStepIsZero(Span span)
        => Except(ExceptionType.MathError, "Range doesn't step much", span);

    internal void CannotIndex(string type, Span span)
        => Except(ExceptionType.IndexError, $"Cannot perform indexing on '{type}'", span);

    internal void CannotIndexWithType(string type1, string type2, Span span)
        => Except(ExceptionType.IndexError, $"Cannot perform indexing on '{type1}' with type '{type2}'", span);

    internal void IndexOutOfBoundary(Span span)
        => Except(ExceptionType.IndexError, $"Index is out of boundary", span);

    internal void CannotAssignToConst(string constant, Span span)
        => Except(ExceptionType.SymbolError, $"Cannot assign to constant '{constant}'", span);

    internal void UseOfUndefined(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is not assigned to yet", span);

    internal void NullReference(Span span)
        => Except(ExceptionType.SymbolError, $"Dereference of a null reference", span);

    internal void UndefinedName(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is not defined", span);

    internal void AlreadyDefined(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is already defined", span);

    internal void AlreadyConstant(string name, Span span)
        => Except(ExceptionType.SymbolError, $"Name '{name}' is already a constant", span);

    internal void InvalidTypeClause(Span span)
        => Except(ExceptionType.SymbolError, $"Type is invalid", span);


    internal void SourcePathNotFound(string path)
        => Except(ExceptionType.SExError, $"File path \"{path}\" not found", new(Position.Subposition));

    internal void InternalError(string? obj, string message)
        => Except(ExceptionType.SExError, $"An internal error has occurred{(obj is null ? "" : $" at {obj}")}, with message:\n{message}", new(Position.Subposition));
}
