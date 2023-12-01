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

    public void Flush()
        => Names.Clear();

    //=====================================================================//

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


    //=====================================================================//

    public ValType ResolveType(string name)
    {
        if (Names.ContainsKey(name))
            return Names[name].Type;

        if (Types.ContainsKey(name))
            return Types[name];

        if (Parent is not null)
            return Parent.ResolveType(name);

        return ValType.Unknown;
    }

    public bool TryResolveType(string name, out ValType value)
    {
        if (Names.ContainsKey(name))
        {
            value = Names[name].Type;
            return true;
        }

        if (Types.ContainsKey(name))
        {
            value = Types[name];
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolveType(name, out value);

        value = ValType.Unknown;
        return false;
    }

    //=====================================================================//

    public LiteralValue Resolve(string name, Span span)
    {
        if (Names.ContainsKey(name))
            return Names[name];
        
        if (Parent is not null)
            return Parent.Resolve(name, span);

        Except($"Name '{name}' is not defined", span);
        return UnknownValue.Template;
    }

    public bool TryResolve(string name, out LiteralValue value)
    {
        if (Names.ContainsKey(name))
        {
            value = Names[name];
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolve(name, out value);

        value = UnknownValue.Template;
        return false;
    }

    //=====================================================================//

    public void PreDeclare(string name, ValType type)
        => Types[name] = type;

    public void PostDeclare(string name)
        => Types.Remove(name);

    //=====================================================================//

    public void Declare(SemanticDeclarationStatement ds, LiteralValue value)
    {
        if (Names.ContainsKey(ds.Name.Value))
            Except($"Name '{ds.Name.Value}' is already declared", ds.Span);

        else if (IsAssignable(ds.TypeHint, value.Type))
        {
            if (ds.IsConstant)
                    Consts.Add(ds.Name.Value);

            Names[ds.Name.Value] = value;
        }
        else
            Except($"Can't assign type '{value.Type.str()}' to '{ds.TypeHint.str()}'", ds.Expression!.Span);

        PostDeclare(ds.Name.Value);
    }

    public void MakeConst(NameLiteral name)
    {
        if (Consts.Contains(name.Value))
            Except($"Name '{name.Value}' is already a constant", name.Span);
        else
            Consts.Add(name.Value);
    }

    public void Assign(SemanticName name, LiteralValue value, bool skipDeclaration = false, Span? valueSpan = null)
    {
        if (Names.ContainsKey(name.Value))
        {
            if (Consts.Contains(name.Value))
                Except($"Can't reassign to constant '{name.Value}'", name.Span);

            else if (TryResolveType(name.Value, out ValType type) && !IsAssignable(type, value.Type))
                Except($"Can't assign type '{value.Type.str()}' to '{type.str()}'", valueSpan ?? name.Span);

            else if (type == ValType.List && !IsAssignable(((ListValue) Names[name.Value]).ElementType, ((ListValue) value).ElementType))
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
