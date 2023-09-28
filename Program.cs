using System.Text;
using SEx;
using SEx.Symbols.Types;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}

REPL repl = new();
var a = new FloatValue(5E51);
Console.WriteLine(a);
while (true)
    repl.Start();
