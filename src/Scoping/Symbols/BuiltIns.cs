using System.Reflection;
using SEx.Evaluation.Conversions;
using SEx.Evaluation.Values;
using SEx.Generic.Logic;
using SEx.SemanticAnalysis;

namespace SEx.Scoping.Symbols;

public static class BuiltIn
{
    public static readonly BuiltinFunctionValue Clear       = new("Clear", TypeSymbol.Void);
    public static readonly BuiltinFunctionValue Print       = new("Print", TypeSymbol.Void,      new NameSymbol("value", TypeSymbol.Any));
    public static readonly BuiltinFunctionValue Read        = new("Read" , TypeSymbol.String,    new NameSymbol("value", TypeSymbol.String));
    public static readonly BuiltinFunctionValue Floor       = new("Floor", TypeSymbol.Integer,   new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Round       = new("Round", TypeSymbol.Integer,   new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Ceiling     = new("Ceiling", TypeSymbol.Integer, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue Absolute    = new("Absolute", TypeSymbol.Number, new NameSymbol("value", TypeSymbol.Number));
    public static readonly BuiltinFunctionValue RandomInt   = new("RandomInt", TypeSymbol.Integer);
    public static readonly BuiltinFunctionValue RandomFloat = new("RandomFloat", TypeSymbol.Float);
    public static readonly BuiltinFunctionValue LengthOf    = new("LengthOf", TypeSymbol.Integer, new NameSymbol("iterable", TypeSymbol.Any));

    public static BuiltinFunctionValue[] GetFunctions()
        => typeof(BuiltIn).GetFields(BindingFlags.Public | BindingFlags.Static)
                          .Where(f => f.FieldType == typeof(BuiltinFunctionValue))
                          .Select(f => (BuiltinFunctionValue) f.GetValue(null)!).ToArray();

    internal static class Backend
    {
        private static VoidValue Clear()
        {
            Console.Clear();
            return VoidValue.Template;
        }

        private static VoidValue Print(string value)
        {
            Console.WriteLine(value.Unescape());
            return VoidValue.Template;
        }

        private static StringValue Read(string value)
        {
            Console.Write(value.Unescape());
            return new(Console.ReadLine() ?? "");
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
            if (func == BuiltIn.Clear)
                return Clear();

            else if (func == BuiltIn.Print)
                return Print((string) Converter.Convert(ConversionKind.AnyToString, args[0], TypeSymbol.String).Value);

            else if (func == BuiltIn.Read)
                return Read((string) args[0].Value);

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

            else if (func == LengthOf)
                return new IntegerValue(IEnumerableValue.GetIterator(args[0])?.Count() ?? 0);

            else
                throw new Exception("Unknown builtin");
        }

    }
}