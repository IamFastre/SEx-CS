using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace SEx.Generic.Logic;

internal static class StringExtension
{
    public static string Escape(this string str) => SymbolDisplay.FormatLiteral(str, false);
    public static string Unescape(this string str) => Regex.Unescape(str);

    public static string Repeat(this string text, int n)
    {
        var arr = new char[text.Length * n];
        for (var i = 0; i < n; i++)
        {
            text.CopyTo(0, arr, i * text.Length, text.Length);
        }
        
        return new string(arr);
    }

    public static string ReplaceFirst(this string str, string term, string replace)
    {
        int position = str.IndexOf(term);
        if (position < 0)
            return str;

        return str[..position] + replace + str[(position + term.Length)..];
    }
}
