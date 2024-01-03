using System.Text;
using System.Text.RegularExpressions;

namespace SEx.Generic.Logic;

internal static class StringExtension
{
    public static string Escape(this string str)
    {
        var builder = new StringBuilder();
        foreach (var c in str)
            switch (c)
            {
                case '\0':
                    builder.Append("\\0");
                    break;
                case '\a':
                    builder.Append("\\a");
                    break;
                case '\b':
                    builder.Append("\\b");
                    break;
                case '\f':
                    builder.Append("\\f");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                case '\v':
                    builder.Append("\\v");
                    break;
                case char when char.IsControl(c):
                    builder.Append(@$"\x{(int)c:X2}");
                    break;
                default:
                    builder.Append(c);
                    break;
            }

        return builder.ToString();
    }

    public static string Unescape(this string str) => Regex.Unescape(str);

    public static string Repeat(this string text, int n)
    {
        var arr = new char[text.Length * n];
        for (var i = 0; i < n; i++)
            text.CopyTo(0, arr, i * text.Length, text.Length);
        
        return new(arr);
    }
}
