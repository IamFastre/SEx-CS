using SEx.AST;
using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Text;
using SEx.Semantics;

namespace SEx.Namespaces;

internal class Scope
{
    public Diagnostics  Diagnostics { get; }
    public Scope?       Parent      { get; }
    public List<string> Consts      { get; }
    public Dictionary<string, LiteralValue> Names { get; }

    public LiteralValue this[string key, Span span] => Resolve(key, span);
    public LiteralValue this[Name name]             => Resolve(name);
    public LiteralValue this[SemanticName name]     => Resolve(name);

    public Scope(Diagnostics? diagnostics = null, Scope? parent = null)
    {
        Diagnostics = diagnostics ?? new();
        Parent      = parent;
        Names       = new();
        Consts      = new();

        DefineDefaults();
    }

    public void Except(string message,
                       Span span,
                       ExceptionType type = ExceptionType.SymbolError,
                       ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Scope);

    public bool Contains(string value)      => Names.ContainsKey(value);
    public bool Contains(Name name)         => Names.ContainsKey(name.Value);
    public bool Contains(SemanticName name) => Names.ContainsKey(name.Value);

    public void Assign(Name name, LiteralValue value)
    {
        if (Consts.Contains(name.Value))
            Except($"Can't reassign to constant '{name.Value}'", name.Span);

        else if (Contains(name.Value) && Names[name.Value].Type != value.Type && Names[name.Value].Type != ValType.Null)
            Except($"Can't assign type '{Names[name.Value].Type.str()}' to '{value.Type.str()}'", name.Span);

        else
            Names[name.Value] = value;
    }

    public LiteralValue Resolve(Name name)         => Resolve(name.Value, name.Span);
    public LiteralValue Resolve(SemanticName name) => Resolve(name.Value, name.Span);
    public LiteralValue Resolve(string value, Span span)
    {
        if (Contains(value))
            return Names[value];
        
        if (Parent is not null)
            return Parent.Resolve(value, span);

        Except($"Name '{value}' is not defined", span);
        return UnknownValue.Template;
    }

    public ValType ResolveType(Name name)
        => Contains(name.Value) ? Names[name.Value].Type : ValType.Unknown;


    public void DefineDefaults()
    {
        Names["SEx"] = new StringValue("awesome!");
        Consts.Add("SEx");
    }
}
