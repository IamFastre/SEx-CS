using SEx.Generic.Constants;

namespace SEx;

public static class SEx
{
    private static readonly string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
    private static readonly string documents    = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public const string Name = "SEx";

    public static bool   IsValentine => DateTime.Now.Month == 2 && DateTime.Now.Day == 14;
    public static string PINK        => C.RGB(255, 70, 130) + C.BLINK;

    public class Path
    {
        public static readonly string Main = $"{System.IO.Path.Combine(documents, Name)}";
        public static readonly string Logs = $"{System.IO.Path.Combine(Main, "Logs")}";

        public static void Prepare()
        {
            if (!Directory.Exists(Main))
                Directory.CreateDirectory(Main);

            if (!Directory.Exists(Logs))
                Directory.CreateDirectory(Logs);
        }

        public override string ToString() => Main;
    }
} 
