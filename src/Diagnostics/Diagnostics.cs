using SEx.Generic.Text;

namespace SEx.Diagnose;

public class Diagnostics
{
    public List<SyntaxException> Exceptions = new();

    public void Flush()
        => Exceptions.Clear();

    public void Add(ExceptionType type, string text, Span span, ExceptionInfo? info = null)
        => Exceptions.Add(new SyntaxException(type, text, span, info));

    public void Throw(Source source)
    {
        foreach (var er in Exceptions)
            er.Print(source.Name, source.Text);
    }
}
