using SEx.Main.REPL;
using SEx.Main.Files;
using System.Text;

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding  = Encoding.Unicode;
}

if (args.Length > 0 && !args[0].StartsWith('-'))
    SExFile.Run(args);
else
    REPL.Run(args);
