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
    public LiteralValue this[Name name]             => Resolve(name);
    public LiteralValue this[SemanticName name]     => Resolve(name);

    public Scope(Diagnostics? diagnostics = null, Scope? parent = null)
    {
        Diagnostics = diagnostics ?? new();
        Parent      = parent;
        Consts      = new();
        Names       = new();
        Types       = new();

        DefineDefaults();
    }

    public void Except(string message,
                       Span span,
                       ExceptionType type = ExceptionType.SymbolError,
                       ExceptionInfo? info = null)
        => Diagnostics.Add(type, message, span, info ?? ExceptionInfo.Scope);

    public bool Contains(string value)      => Names.ContainsKey(value)      || Parent?.Contains(value) is not null or false;
    public bool Contains(Name name)         => Names.ContainsKey(name.Value) || Parent?.Contains(name)  is not null or false;
    public bool Contains(SemanticName name) => Names.ContainsKey(name.Value) || Parent?.Contains(name)  is not null or false;

    public void Flush()
        => Names.Clear();

    public void DefineDefaults()
    {
        Names["SEx"] = new StringValue("awesome!");
        Consts.Add("SEx");
    }

    public LiteralValue TryResolve(string value)
    {
        if (Contains(value))
            return Names[value];
        
        if (Parent is not null)
            return Parent.TryResolve(value);

        return UnknownValue.Template;
    }

    public ValType ResolveType(Name name)
    {
        if (Contains(name))
            return this[name].Type;

        if (Types.ContainsKey(name.Value))
            return Types[name.Value];

        if (Parent is not null)
        {
            if (Parent.Contains(name))
                return Parent[name].Type;
            
            if (Parent.Types.ContainsKey(name.Value))
                return Parent.Types[name.Value];
        }

        return ValType.Unknown;
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

    internal void Declare(SemanticDeclarationStatement dec, LiteralValue value)
    {
        Types.Remove(dec.Name.Value);

        if (dec.IsConstant && dec.Expression is null)
        {
            if (Contains(dec.Name))
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
        else
        {
            if (dec.IsConstant)
                Consts.Add(dec.Name.Value);

            Names[dec.Name.Value] = value;
        }
    }

    public void Assign(Name name, LiteralValue value)
    {
        Types.Remove(name.Value);

        if (!Contains(name))
            Except($"Name '{name.Value}' was not declared to assign to", name.Span);

        else if (Consts.Contains(name.Value))
            Except($"Can't reassign to constant '{name.Value}'", name.Span);

        else if (Contains(name.Value) && Names[name.Value].Type != value.Type && Names[name.Value].Type != ValType.Null)
            Except($"Can't assign type '{Names[name.Value].Type.str()}' to '{value.Type.str()}'", name.Span);

        else
            Names[name.Value] = value;
    }
}
