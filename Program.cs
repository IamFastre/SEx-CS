using System.Text;
using Util;
using Lexing;
using Parsing;
using AST;
using Diagnosing;

string line;
Diagnostics diagnostics;
Parser parser;

while (true)
{
    Console.Write(">> ");
    line = Console.ReadLine()!;
    diagnostics = Diagnostics.REPL(line);
    parser = new(line, diagnostics);

    Console.WriteLine();
    diagnostics.Throw();

    Console.WriteLine("\nTokens:");
    foreach (var tk in parser.Tokens)
        Console.WriteLine(tk);

    Console.WriteLine("\nNodes:");
    foreach (var nd in parser.Tree)
        Console.WriteLine(nd);
}