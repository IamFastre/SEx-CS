using SEx.Evaluate.Values;
using SEx.Scoping.Symbols;

namespace SEx.Scoping;

public class Scope
{
    public Scope? Parent { get; }
    public Dictionary<NameSymbol, LiteralValue> Names { get; }

    public bool IsGlobal => Parent is null;
    public LiteralValue this[NameSymbol variable] => Names[variable];

    public Scope(Scope? parent = null)
    {
        Parent = parent;
        Names  = new();

        if (parent is null)
            DeclareBuiltIns();
    }

    private void DeclareBuiltIns()
    {
        foreach (var func in BuiltIn.GetFunctions())
            Declare(func.GetSymbol(), func);
    }

    public void AddVariable(NameSymbol variable, LiteralValue value)
        => Names.Add(variable, value);

    public void EditVariable(NameSymbol variable, LiteralValue value)
    {
        if (Names.ContainsKey(variable))
            Names[variable] = value;
        else
            throw new ArgumentException("This item does not exits");
    }

    public void Flush()
        => Names.Clear();

    public SemanticScope ToSemantic()
        => new(Names.Keys);

    //=====================================================================//
    //=====================================================================//

    public bool TryResolve(NameSymbol variable, out LiteralValue value)
    {
        if (Names.ContainsKey(variable))
        {
            value = Names[variable];
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolve(variable, out value);

        value = UnknownValue.Template;
        return false;
    }

    public void Declare(NameSymbol variable, LiteralValue value)
    {
        if ((variable.Type, value.Type).IsAssignable())
            AddVariable(variable, value);
    }

    public void MakeConstant(string name)
    {
        foreach (var sym in Names.Keys)
            if (sym.Name == name)
                sym.MakeConstant();
    }

    public void Assign(NameSymbol variable, LiteralValue value, bool force = false)
    {
        if (force)
            Names[variable] = value;

        if (Names.ContainsKey(variable))
                EditVariable(variable, value);

        else if (Parent is not null && Parent.Names.ContainsKey(variable))
            Parent.Assign(variable, value);
    }
}
