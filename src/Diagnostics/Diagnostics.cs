using SEx.Generic.Text;

namespace SEx.Diagnose;

public class Diagnostics
{
    public List<SyntaxException> Exceptions = new();

    public void Flush()
        => Exceptions.RemoveAll((SyntaxException e) => e is not null);

    public void Add(ExceptionType type, string text, Span span, ExceptionInfo? info = null)
        => Exceptions.Add(new SyntaxException(type, text, span, info));
}
