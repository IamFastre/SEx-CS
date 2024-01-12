using SEx.Evaluate.Values;
using SEx.Generic.Constants;
using SEx.Scoping.Symbols;

namespace SEx.Scoping;

public class Scope
{
    public Scope?                      Parent { get; }
    public Dictionary<NameSymbol, int> Names  { get; } = new();
    public List<LiteralValue?>         Values { get; } = new();

    public bool IsGlobal => Parent is null;

    public LiteralValue this[NameSymbol variable] => Get(variable);

    public Scope(Scope? parent = null)
    {
        Parent = parent;

        if (parent is null)
            DeclareBuiltIns();
    }

    private void DeclareBuiltIns()
    {
        Set(new(CONSTS.NULL),  new NullValue(),      true);
        Set(new(CONSTS.TRUE),  new BoolValue(true),  true);
        Set(new(CONSTS.FALSE), new BoolValue(false), true);

        foreach (var func in BuiltIn.GetFunctions())
            Set(func.GetSymbol(), func, true);
    }

    public void Flush()
    {
        Names.Clear();
        Values.Clear();

        if (Parent is null)
            DeclareBuiltIns();
    }

    //=====================================================================//
    //=====================================================================//

    public void Set(NameSymbol name, LiteralValue value, bool declare = false)
    {
        if (!(name.Type, value.Type).IsAssignable())
            goto Return;

        if (!(Names.ContainsKey(name) || declare))
            throw new IndexOutOfRangeException($"Name {name} isn't added");

        if (!name.Type.IsMutable)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                var v = Values[i];
                if (value.Equals(v) || v is null)
                {
                    if (v is null)
                        Values[i] = value;

                    Names[name] = i;
                    goto Return;
                }
            }
        }

        Values.Add(value);
        Names[name] = Values.Count - 1;

        Return:
            CleanUp();
            return;
    }

    public void Point(NameSymbol old, NameSymbol @new, bool declare = false)
    {
        if (!(old.Type, @new.Type).IsAssignable())
            goto Return;

        if (!(Names.ContainsKey(old) || declare))
            throw new IndexOutOfRangeException($"Name {old} isn't added");

        var index = Names[old];
        Names.Add(@new, index);

        Return:
            CleanUp();
            return;
    }

    public void Mod(NameSymbol name, LiteralValue value)
    {
        var index = Names[name];
        Values[index] = value;

        CleanUp();
    }

    public LiteralValue Get(NameSymbol name)
    {
        if (Names.ContainsKey(name))
            return Values[Names[name]]!;
        
        return UnknownValue.Template;
    }

    public void CleanUp()
    {
        for (int i = 0; i < Values.Count; i++)
            if (!Names.ContainsValue(i))
                Values[i] = null;
    }

    //=====================================================================//
    //=====================================================================//

    public bool TryResolve(NameSymbol name, out LiteralValue value)
    {
        if (Names.ContainsKey(name))
        {
            value = Get(name);
            return true;
        }

        if (Parent is not null)
            return Parent.TryResolve(name, out value);

        value = UnknownValue.Template;
        return false;
    }

    public void Assign(NameSymbol variable, LiteralValue value, bool force = false)
    {
        if (Names.ContainsKey(variable) || force)
        {
            if (Names.ContainsKey(variable) && value.Type.IsMutable)
                Mod(variable, value);
            else
                Set(variable, value, force);
        }

        else if (Parent is not null && Parent.Names.ContainsKey(variable))
            Parent.Assign(variable, value);
    }

    public void MakeConstant(string name)
    {
        foreach (var sym in Names.Keys)
            if (sym.Name == name)
            {
                sym.MakeConstant();
                return;
            }
    }
}
