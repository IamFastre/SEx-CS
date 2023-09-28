using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace SEx.Generic.Logic;

internal static class StringExtension
{
    public static string ToLiteral(this string str) => SymbolDisplay.FormatLiteral(str, false);
    public static string ToEscaped(this string str) => Regex.Unescape(str);
}
