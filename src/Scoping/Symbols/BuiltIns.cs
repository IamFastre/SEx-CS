using System.Reflection;
using SEx.Evaluate.Conversions;
using SEx.Evaluate.Values;
using SEx.Semantics;

namespace SEx.Scoping.Symbols;

public static class BuiltIn
{
    public static readonly FunctionSymbol Print       = new("print", TypeSymbol.Void, true, new ParameterSymbol("value", TypeSymbol.Any));
    public static readonly FunctionSymbol Add         = new("add", TypeSymbol.Integer, true, new ParameterSymbol("arg1", TypeSymbol.Integer), new ParameterSymbol("arg2", TypeSymbol.Integer));
    public static readonly FunctionSymbol Floor       = new("floor", TypeSymbol.Integer, true, new ParameterSymbol("value", TypeSymbol.Number));
    public static readonly FunctionSymbol Round       = new("round", TypeSymbol.Integer, true, new ParameterSymbol("value", TypeSymbol.Number));
    public static readonly FunctionSymbol Ceiling     = new("ceiling", TypeSymbol.Integer, true, new ParameterSymbol("value", TypeSymbol.Number));
    public static readonly FunctionSymbol Absolute    = new("absolute", TypeSymbol.Number, true, new ParameterSymbol("value", TypeSymbol.Number));
    public static readonly FunctionSymbol RandomInt   = new("randomInt", TypeSymbol.Integer, true);
    public static readonly FunctionSymbol RandomFloat = new("randomFloat", TypeSymbol.Float, true);

    public static FunctionSymbol[] GetFunctions()
        => typeof(BuiltIn).GetFields(BindingFlags.Public | BindingFlags.Static)
                          .Where(f => f.FieldType == typeof(FunctionSymbol))
                          .Select(f => (FunctionSymbol) f.GetValue(null)!).ToArray();

    internal static class Backend
    {
        private static VoidValue Print(string value)
        {
            Console.WriteLine(value);
            return VoidValue.Template;
        }

        private static IntegerValue Add(double value1, double value2)
            => new(value1 + value2);

        private static IntegerValue Floor(double value)
            => new(Math.Floor(value));

        private static IntegerValue Round(double value)
            => new(Math.Round(value));

        private static IntegerValue Ceiling(double value)
            => new(Math.Ceiling(value));

        private static NumberValue Absolute(double value)
            => NumberValue.Get(Math.Abs(value));

        private static IntegerValue RandomInt()
            => new(new Random().NextDouble() * 1e16);

        private static FloatValue RandomFloat()
            => new(new Random().NextDouble());



        public static LiteralValue Evaluate(FunctionValue func, LiteralValue[] args)
        {
            if (func.Symbol == BuiltIn.Print)
                return Print((string) Converter.Convert(ConversionKind.AnyToString, args[0], TypeSymbol.String).Value);

            else if (func.Symbol == BuiltIn.Add)
                return Add((double) args[0].Value, (double) args[1].Value);

            else if (func.Symbol == BuiltIn.Floor)
                return Floor((double) args[0].Value);

            else if (func.Symbol == BuiltIn.Round)
                return Round((double) args[0].Value);

            else if (func.Symbol == BuiltIn.Ceiling)
                return Ceiling((double) args[0].Value);

            else if (func.Symbol == BuiltIn.Absolute)
                return Absolute((double) args[0].Value);

            else if (func.Symbol == BuiltIn.RandomInt)
                return RandomInt();

            else if (func.Symbol == BuiltIn.RandomFloat)
                return RandomFloat();

            else
                throw new Exception("Unknown builtin");
        }

    }
}