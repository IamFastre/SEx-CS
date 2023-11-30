using SEx.AST;
using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Semantics;

namespace SEx.Scoping;

internal class Scope
{
    public Diagnostics  Diagnostics { get; }
    public Scope?       Parent      { get; }
    public List<string> Consts      { get; }
    public Dictionary<string, LiteralValue> Names { get; }
    public Dictionary<string, ValType>      Types { get; }

    public LiteralValue this[string key, Span span] => Resolve(key, span);
    public LiteralValue this[NameLiteral name]      => Resolve(name);
    public LiteralValue this[SemanticName name]     => Resolve(name);

    public Scope(Diagnostics? diagnostics = null, Scope? parent = null)
    {
        Diagnostics = diagnostics ?? new();
        Parent      = parent;
        Consts      = new();
        Names       = new();
        Types       = new();
    }

    public void Except(string message,
                       Span span,
                       ExceptionType type = ExceptionType.SymbolError,
                       ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Scope);

    public bool Contains(string value)      => Names.ContainsKey(value)      || Parent?.Contains(value) is not null or false;
    public bool Contains(NameLiteral name)  => Names.ContainsKey(name.Value) || Parent?.Contains(name)  is not null or false;
    public bool Contains(SemanticName name) => Names.ContainsKey(name.Value) || Parent?.Contains(name)  is not null or false;

    public void Flush()
        => Names.Clear();

    public static bool IsAssignable(ValType value1, ValType value2, bool noOrder)
    {
        if (!noOrder)
            return IsAssignable(value1, value2);
        
        return IsAssignable(value1, value2) || IsAssignable(value2, value1);
    }

    public static bool IsAssignable(ValType hint, ValType value)
        => hint.HasFlag(value)
        || ValType.Nones.HasFlag(value)
        || ValType.Nones.HasFlag(hint);

    public LiteralValue TryResolve(string value)
    {
        if (Names.ContainsKey(value))
            return Names[value];
        
        if (Parent is not null)
            return Parent.TryResolve(value);

        return UnknownValue.Template;
    }

    public ValType ResolveType(NameLiteral name)
        => ResolveType(new SemanticName(name, ValType.Unknown));

    public ValType ResolveType(SemanticName name)
    {
        if (Names.ContainsKey(name.Value))
            return this[name].Type;

        if (Types.ContainsKey(name.Value))
            return Types[name.Value];

        if (Parent is not null)
            return Parent.ResolveType(name);

        return ValType.Unknown;
    }

    public LiteralValue Resolve(NameLiteral name)  => Resolve(name.Value, name.Span);
    public LiteralValue Resolve(SemanticName name) => Resolve(name.Value, name.Span);
    public LiteralValue Resolve(string value, Span span)
    {
        if (Names.ContainsKey(value))
            return Names[value];
        
        if (Parent is not null)
            return Parent.Resolve(value, span);

        Except($"Name '{value}' is not defined", span);
        return UnknownValue.Template;
    }

    public void Declare(SemanticDeclarationStatement dec, LiteralValue value)
    {
        if (dec.IsConstant && dec.Expression is null)
        {
            if (Names.ContainsKey(dec.Name.Value))
            {
                if (dec.Type is not null)
                        Except($"No need for added type '{dec.Type.Value}' in constant construction",
                               dec.Type.Span,ExceptionType.SyntaxError);

                if (!Consts.Contains(dec.Name.Value))
                    Consts.Add(dec.Name.Value);
            }
            else
                Except($"No value was given to constant '{dec.Name.Value}'", dec.Span);
        }
        else if (Contains(dec.Name))
            Except($"Name '{dec.Name.Value}' is already declared", dec.Span);
        else if (!ValType.UAVT.HasFlag(value.Type))
        {
            if (!IsAssignable(dec.TypeHint, value.Type))
            {
                Except($"Can't assign type '{value.Type.str()}' to '{dec.TypeHint.str()}'", dec.Expression!.Span);
                return;
            }

            if (dec.IsConstant)
                Consts.Add(dec.Name.Value);

            Names[dec.Name.Value] = value;
        }
    }

    public void Assign(SemanticName name, LiteralValue value, bool skipDeclaration = false, Span? valueSpan = null)
    {
        if (Names.ContainsKey(name.Value))
        {
            if (Consts.Contains(name.Value))
                Except($"Can't reassign to constant '{name.Value}'", name.Span);

            else if (Names.ContainsKey(name.Value) && !IsAssignable(ResolveType(name), value.Type))
                Except($"Can't assign type '{value.Type.str()}' to '{ResolveType(name).str()}'", valueSpan ?? name.Span);

            else if (ResolveType(name) == ValType.List && !IsAssignable(((ListValue) Names[name.Value]).ElementType, ((ListValue) value).ElementType))
                Except($"Cannot assign list of type '{((ListValue) Names[name.Value]).ElementType.str()}' to '{((ListValue) value).ElementType.str()}'", valueSpan ?? name.Span);

            else
                Names[name.Value] = value;
        }

        else if (Parent is not null && Parent.Names.ContainsKey(name.Value))
            Parent.Assign(name, value, skipDeclaration, valueSpan);

        else
            Except($"Name '{name.Value}' was not declared to assign to", name.Span);
    }
}
