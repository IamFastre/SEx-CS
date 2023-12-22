using System.Reflection;
using SEx.Evaluate.Conversions;
using SEx.Evaluate.Values;
using SEx.Semantics;

namespace SEx.Scoping.Symbols;

public static class BuiltIn
{
    public static readonly BuiltinFunctionValue Print       = new("print", TypeSymbol.Void, new NameSymbol("value", TypeSymbol.Any));
    public static readonly BuiltinFunctionValue Floor       = new("floor", TypeSymbol.Integer, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Round       = new("round", TypeSymbol.Integer, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Ceiling     = new("ceiling", TypeSymbol.Integer, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Absolute    = new("absolute", TypeSymbol.Number, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue RandomInt   = new("randomInt", TypeSymbol.Integer);
    public static readonly BuiltinFunctionValue RandomFloat = new("randomFloat", TypeSymbol.Float);

    public static BuiltinFunctionValue[] GetFunctions()
        => typeof(BuiltIn).GetFields(BindingFlags.Public | BindingFlags.Static)
                          .Where(f => f.FieldType == typeof(BuiltinFunctionValue))
                          .Select(f => (BuiltinFunctionValue) f.GetValue(null)!).ToArray();

    internal static class Backend
    {
        private static VoidValue Print(string value)
        {
            Console.WriteLine(value);
            return VoidValue.Template;
        }

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
            if (func == BuiltIn.Print)
                return Print((string) Converter.Convert(ConversionKind.AnyToString, args[0], TypeSymbol.String).Value);

            else if (func == BuiltIn.Floor)
                return Floor((double) args[0].Value);

            else if (func == BuiltIn.Round)
                return Round((double) args[0].Value);

            else if (func == BuiltIn.Ceiling)
                return Ceiling((double) args[0].Value);

            else if (func == BuiltIn.Absolute)
                return Absolute((double) args[0].Value);

            else if (func == BuiltIn.RandomInt)
                return RandomInt();

            else if (func == BuiltIn.RandomFloat)
                return RandomFloat();

            else
                throw new Exception("Unknown builtin");
        }

    }
}