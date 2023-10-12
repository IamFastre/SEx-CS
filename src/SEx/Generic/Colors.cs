namespace SEx.Generic;

public class C
{
    public static readonly string END        = "\u001b[0m";
    public static readonly string BOLD       = "\u001b[1m";
    public static readonly string DIM        = "\u001b[2m";
    public static readonly string ITALIC     = "\u001b[3m";
    public static readonly string UNDERLINE  = "\u001b[4m";
    public static readonly string BLINK      = "\u001b[5m";
    public static readonly string BLINK2     = "\u001b[6m";
    public static readonly string SELECTED   = "\u001b[7m";
    public static readonly string INVISIBLE  = "\u001b[8m";
    public static readonly string STRIKE     = "\u001b[9m";
    public static readonly string UNDERLINE2 = "\u001b[21m";

    public static readonly string WHITE      = "\u001b[37m";
    public static readonly string BLACK      = "\u001b[30m";
    public static readonly string RED        = "\u001b[31m"; 
    public static readonly string RED2       = "\u001b[91m";
    public static readonly string YELLOW     = "\u001b[33m";
    public static readonly string YELLOW2    = "\u001b[93m";
    public static readonly string GREEN      = "\u001b[32m";
    public static readonly string GREEN2     = "\u001b[92m";
    public static readonly string BLUE       = "\u001b[34m";
    public static readonly string BLUE2      = "\u001b[94m";
    public static readonly string VIOLET     = "\u001b[35m";
    public static readonly string VIOLET2    = "\u001b[95m";
 
    public static string RGB(int R, int G, int B, bool isBackground = false)
    {
        var BG = isBackground ? 48 : 38;
        return $"\u001b[{BG};2;{R};{G};{B}m";
    }
}