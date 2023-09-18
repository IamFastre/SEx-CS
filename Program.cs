using System.Text;
using SEx.Parse;
using SEx.Analysis;
using SEx.Evaluate;

Console.OutputEncoding = Encoding.UTF8;

string line;
Diagnostics diagnostics;
Parser parser;
Evaluator result;

while (true)
{
    Console.Write(">> ");
    line = Console.ReadLine()!;
    diagnostics = new();
    parser = new(line, diagnostics);

    Console.WriteLine("\nTokens:");
    foreach (var tk in parser.Tokens)
        Console.WriteLine($"• {tk}");

    Console.WriteLine("\nTree:\n> " + parser.Tree);

    Console.WriteLine("\nLexer & Parser Diagnostics:");
        foreach (var er in parser.Diagnostics.Exceptions)
            Console.WriteLine(er);

    try
    {
        result = new Evaluator(line, diagnostics);
        Console.WriteLine("\nEvaluator Diagnostics:");
            foreach (var er in result.Diagnostics.Exceptions)
                Console.WriteLine(er);

        Console.WriteLine($"\nValue <{result.Value?.Type}>:");
        if (result.Value?.Value is not null)
        {
            string[] str = result.Value.Value.ToString().Split('\n');
            foreach (string l in str)
                Console.WriteLine($"> {l}");
        } else {
            Console.WriteLine("> Null");
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine($"{exception}");
    }
}