using System.Text;
using SEx;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}
Console.CancelKeyPress += (sender, args) => {
    args.Cancel = true;
    Console.WriteLine();
};
REPL repl = new();

while (true)
    repl.Start();
