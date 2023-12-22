using SEx.Generic.Constants;
using SEx.Scoping.Symbols;
using SEx.Semantics;

namespace SEx.Evaluate.Values;

public class FunctionValue : LiteralValue
{
    public string            Name       { get; }
    public NameSymbol[] Parameters { get; }
    public SemanticStatement Body       { get; }

    public bool IsBuiltin => Body is null;

    public override object Value => null!;
    public override GenericTypeSymbol Type { get; }


    public FunctionValue(string name, NameSymbol[] parameters, TypeSymbol returnType, SemanticStatement body)
    {
        Name       = name;
        Parameters = parameters;
        Body       = body;
        Type       = TypeSymbol.Function(returnType, parameters.Select(p => p.Type).ToArray());
    }

    public override string ToString()
        => $"{C.CYAN}{GetString()}{C.END}";

    public override bool Equals(object? obj) => obj is FunctionValue fv && GetHashCode() == fv.GetHashCode();
    public override int  GetHashCode()       => HashCode.Combine(Parameters, Body, Type);

    public override string GetString()
    {
        var str = $"{Name}(";

        for (int i = 0; i < Parameters.Length; i++)
        {
            var name = Parameters[i].ToString();
            var type = Parameters[i].Type.ToString();

            str += $"{name}:{type}";
            if (i != Parameters.Length - 1)
                str += ", ";
        }
        str += ")";

        return $"{str} -> {Type.Parameters[0]}";
    }
}

public class BuiltinFunctionValue : FunctionValue
{
    public BuiltinFunctionValue(string name, TypeSymbol returnType, params NameSymbol[] parameters)
        : base(name, parameters, returnType, null!) { }
    
    public NameSymbol GetSymbol() => new(Name, Type, true);
}