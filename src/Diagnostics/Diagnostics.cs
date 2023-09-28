using SEx.Generic;

namespace SEx.Diagnose;

public class Diagnostics
{
    private List<SyntaxException> _exceptions;
    public List<SyntaxException> Exceptions => _exceptions;

    public Diagnostics()
    {
        _exceptions = new();
    }

    public void Add(ExceptionType type, string text, Span span) =>
        _exceptions.Add(new SyntaxException(type, text, span));
}
