using SEx.Diagnose;
using SEx.Evaluate.Values;
using SEx.Generic.Text;

namespace SEx.Scoping;

internal class Scope
{
    public Scope?       Parent      { get; }
    public Dictionary<VariableSymbol, LiteralValue> Variables { get; }

    public LiteralValue this[VariableSymbol variable] => Variables[variable];

    public Scope(Scope? parent = null)
    {
        Parent      = parent;
        Variables   = new();
    }

    public void AddVariable(VariableSymbol variable, LiteralValue value)
        => Variables.Add(variable, value);

    public void EditVariable(VariableSymbol variable, LiteralValue value)
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

    public ValType ResolveType(VariableSymbol variable)
    {
        if (Variables.ContainsKey(variable))
            return Variables[variable].Type;

        if (Parent is not null)
            return Parent.ResolveType(variable);

        return ValType.Unknown;
    }

    public bool TryResolveType(VariableSymbol variable, out ValType value)
    {
        if (Variables.ContainsKey(variable))
        {
            value = Variables[variable].Type;
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolveType(variable, out value);

        value = ValType.Unknown;
        return false;
    }

    //=====================================================================//

    public LiteralValue Resolve(VariableSymbol variable)
    {
        if (Variables.ContainsKey(variable))
            return Variables[variable];
        
        if (Parent is not null)
            return Parent.Resolve(variable);

        return UnknownValue.Template;
    }

    public bool TryResolve(VariableSymbol variable, out LiteralValue value)
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

    public bool IsDeclared(VariableSymbol variable)
        => Variables.ContainsKey(variable);

    public void Declare(VariableSymbol variable, LiteralValue value)
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

    public void Assign(VariableSymbol variable, LiteralValue value)
    {
        if (Variables.ContainsKey(variable))
                EditVariable(variable, value);

        else if (Parent is not null && Parent.Variables.ContainsKey(variable))
            Parent.Assign(variable, value);
    }
}
