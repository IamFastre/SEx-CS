using System.Text;
using Lexing;

Console.OutputEncoding = Encoding.UTF8;


while (true)
{
    string text;
    bool REPL = false;

    if (REPL)
    {
        Console.Write($">> ");
        text = Console.ReadLine()!;
    }
    else
    {
        Console.Write("Read now (enter)?");
        Console.ReadLine();
        text = File.ReadAllText(@"D:\DevShit\C#\SEx-C\test\file.sex", Encoding.UTF8);
    }

    Lexer lexer = new(text);
    lexer.Start();
    foreach (var item in lexer.Tokens)
    {
        Console.WriteLine(item);
    }
}