using SEx.Evaluate.Values;
using SEx.Scoping.Symbols;

namespace SEx.Scoping;

internal class Scope
{
    public Scope? Parent { get; }
    public Dictionary<NameSymbol, LiteralValue> Variables { get; }

    public LiteralValue this[NameSymbol variable] => Variables[variable];

    public Scope(Scope? parent = null)
    {
        Parent      = parent;
        Variables   = new();

        if (parent is null)
            DeclareBuiltIns();
    }

    private void DeclareBuiltIns()
    {
        foreach (var func in BuiltIn.GetFunctions())
            Declare(func, new BuiltinFunctionValue(func));
    }

    public void AddVariable(NameSymbol variable, LiteralValue value)
        => Variables.Add(variable, value);

    public void EditVariable(NameSymbol variable, LiteralValue value)
    {
        if (Variables.ContainsKey(variable))
            Variables[variable] = value;
        else
            throw new ArgumentException("This item does not exits");
    }

    public void Flush()
        => Variables.Clear();

    //=====================================================================//
    //=====================================================================//

    public TypeSymbol ResolveType(NameSymbol variable)
    {
        if (Variables.ContainsKey(variable))
            return Variables[variable].Type;

        if (Parent is not null)
            return Parent.ResolveType(variable);

        return TypeSymbol.Unknown;
    }

    public bool TryResolveType(NameSymbol variable, out TypeSymbol value)
    {
        if (Variables.ContainsKey(variable))
        {
            value = Variables[variable].Type;
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolveType(variable, out value);

        value = TypeSymbol.Unknown;
        return false;
    }

    //=====================================================================//

    public LiteralValue Resolve(NameSymbol variable)
    {
        if (Variables.ContainsKey(variable))
            return Variables[variable];
        
        if (Parent is not null)
            return Parent.Resolve(variable);

        return UnknownValue.Template;
    }

    public bool TryResolve(NameSymbol variable, out LiteralValue value)
    {
        if (Variables.ContainsKey(variable))
        {
            value = Variables[variable];
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolve(variable, out value);

        value = UnknownValue.Template;
        return false;
    }

    //=====================================================================//

    public bool IsDeclared(NameSymbol variable)
        => Variables.ContainsKey(variable);

    public void Declare(NameSymbol variable, LiteralValue value)
    {
        if ((variable.Type, value.Type).IsAssignable())
            AddVariable(variable, value);
    }

    public void MakeConstant(string name)
    {
        foreach (var sym in Variables.Keys)
            if (sym.Name == name)
                sym.MakeConstant();
    }

    public void Assign(NameSymbol variable, LiteralValue value, bool skipDeclaration = false)
    {
        if (skipDeclaration)
            Variables[variable] = value;

        if (Variables.ContainsKey(variable))
                EditVariable(variable, value);

        else if (Parent is not null && Parent.Variables.ContainsKey(variable))
            Parent.Assign(variable, value);
    }
}
