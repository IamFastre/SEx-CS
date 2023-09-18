using SEx.Analysis;
using SEx.Generic;

namespace SEx.Lex;

// An enumeration of the token types possible in the source code
public enum TokenType
{
    Bad,
    Comment,
    EOF,

    WhiteSpace,
    Integer,
    Float,
    Char,
    String,

    Identifier,
    Keyword,
    Null,

    Operator,
    Separator,

    OpenParenthesis,
    CloseParenthesis,
    OpenCurlyBracket,
    CloseCurlyBracket,
    OpenSquareBracket,
    CloseSquareBracket,
}


// The Token class represents a token in a programming language,
// storing its value, type, and position in the source code
public class Token
{
    public string? Value;
    public TokenType Type;
    public Span Span;

    public Token(string? value, TokenType type, Span span)
    {
        Value = value;
        Type  = type;
        Span  = span;
    }

    public static readonly Token Template = new(null, TokenType.Null, Span.Template);

     public override string ToString()
     {
        TokenType[] noVal = {TokenType.WhiteSpace, TokenType.EOF};

        var val = !noVal.Contains(Type) || Value == null ? $": {Value}" : "";

        return $"[{Type}{val}]";
     }

    public string Full => $"{this} at {Span}";
}



// A class responsible for tokenizing a given text into a list of tokens
// based on specific rules and patterns
public class Lexer
{
    private int Index;
    private readonly string Source;
    public List<Token> Tokens;
    public Diagnostics Diagnostics;

    private char Peek(int i = 1)
        => Index+i < Source.Length ? Source[Index+i] : '\0';

    private bool EOF => Index >= Source.Length; // End of file
    private bool EOL => Current == '\n';        // End of line

    private char Current => Peek(0);

    public Lexer(string source, Diagnostics? diagnostics = null)
    {
        // If no diagnostics object is passed, create a new one
        Diagnostics = diagnostics ?? new Diagnostics();
        Source = source;
        Tokens = new();
    }


    /// <summary>
    /// The Start function is a method in C# that is used to initialize and start a program or
    /// application.
    /// </summary>
    public Lexer Initialize()
    {
        while (true)
        {
            var token = GetToken();
            Tokens.Add(token);

            if (token.Type == TokenType.EOF)
                break;

            Index++;
        }

        return this;
    }


    public Token GetToken()
    {
        // Get the starting value and position of the token
        var value = Current.ToString();
        var span  = new Span(GetPosition());

        // It adds to the value, very straight forward
        // The "advance" parameter however determines when to increment
        // that's if to increment at all:
        // • -1 => Before adding to value
        // •  0 => Never
        // •  1 => After adding to value
        // and it returns the value in case it's needed yk
        string AddValue(int advance = 0)
        {
            if (advance == -1) Index++;

            value += Current.ToString();
            span.End = GetPosition();

            if (advance == 1) Index++;

            return value;
        }

        string SyncValue()
        {
            var range = span.Start.Index..(Index+1);

            value    = Source[range];
            span.End = GetPosition();

            return value;
        }

        // If it's a whitespace, just add a whitespace token and advance
        if (char.IsWhiteSpace(Current))
            return new Token(value, TokenType.WhiteSpace, span);

        // If it's a zero terminator return EOF token and advance
        if (Current == '\0' || EOF)
            return new Token(value, TokenType.EOF, span);


        if (AreUpcoming(Checker.BigOprts))
            return new Token(SyncValue(), TokenType.Operator, span);

        // If it's some sort of bracket, this regards whether
        // it's opening or closing for now
        if (Current == '(')
            return new Token(value, TokenType.OpenParenthesis, span);
        if (Current == '[')
            return new Token(value, TokenType.OpenSquareBracket, span);
        if (Current == '{')
            return new Token(value, TokenType.OpenCurlyBracket, span);

        if (Current == ')')
            return new Token(value, TokenType.CloseParenthesis, span);
        if (Current == ']')
            return new Token(value, TokenType.CloseSquareBracket, span);
        if (Current == '}')
            return new Token(value, TokenType.CloseCurlyBracket, span);

        // If it's an operator that can be doubled
        if (Checker.Operators.Contains(Current))
        {
            if (Peek() == '=')
                AddValue(-1);

            return new Token(value, TokenType.Operator, span);
        }

        // If it's a digit or a dot
        // loop until you reach its end and add it as a token and advance
        if (char.IsDigit(Current) || (Current == '.' && char.IsDigit(Peek())))
        {
            int dots() => value.Count(s => s == '.');

            while (char.IsDigit(Peek()) || Peek() == '.' && !(dots() > 0))
            {
                if (Peek() == '.' && !char.IsDigit(Peek(2)))
                    break;
                AddValue(-1);
            }

            // Decide for whether it's a float or integer
            var type =  dots() == 1 ? TokenType.Float : TokenType.Integer;
            return new Token(value, type, span);
        }

        // If it's a letter or an underscore
        // loop until you reach its end and add it as a token and advance
        if (char.IsLetter(Current) || Current == '_')
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                AddValue(-1);
            }

            if (Checker.Keywords.Contains(value))
                return new Token(value, TokenType.Keyword, span);

            return new Token(value, TokenType.Identifier, span);
        }

        // If it's an opening quotation mark
        // loop until you reach its end and add it as a token and advance
        if (Checker.OpnQuotes.Contains(Current))
        {
            // Get the closing quote for that opening one
            char clsQuote = Checker.GetOtherPair(Current);
            Index++;

            while (Current != clsQuote)
            {
                if (EOF || EOL)
                {
                    Diagnostics.Add(ExceptionType.SyntaxError, $"Unterminated string literal", span);
                    return new Token(value, TokenType.Bad, span);
                }

                AddValue(1);
            }

            AddValue();
            return new Token(value, TokenType.String, span);
        }

        if (Current == '\'')
        {
            Index++;

            while (!(Current == '\''))
            {
                if (EOF || EOL)
                {
                    Diagnostics.Add(ExceptionType.SyntaxError, $"Unterminated character literal", span);
                    return new Token(value, TokenType.Bad, span);
                }

                AddValue(1);
            }

            AddValue();
            return new Token(value, TokenType.Char, span);
        }

        // Checking it last as other token types might start with a separator character
        if (Checker.Separators.Contains(Current))
            return new Token(value, TokenType.Separator, span);

        // If no type is met, it's bad
        Diagnostics.Add(ExceptionType.SyntaxError, $"Unrecognized character: {Current}", span);
        return new Token(value, TokenType.Bad, span);
    }


    /// <summary>
    /// The `GetPosition` function returns the line and column position of a given index in a text,
    /// accounting for new line characters
    /// </summary>
    /// <param name="index">The `index` parameter is an optional parameter of type `int?` (nullable
    /// integer). It represents the specific index in the `Text` string that you want to get the position
    /// for. If no specific index is given, it defaults to `null`</param>
    /// <returns>
    /// The method is returning a Position object, which represents a specific position in a text file
    /// </returns>
    private Position GetPosition(int? index = null)
    {
        // If no specific index is given use instance's index
        // and if out of file boundaries use last index
        index ??= !EOF ? this.Index : Source.Length - 1;
        // Line and Column start at one
        int line = 1;
        int column = 1;

        // Loop checking if you hit a new line character
        // if so add one to the line and start over with column
        for (int i = 0; i < index; i++)            
        {
            char character = Source[i];

            if (character == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
        }

        // Return the position and if it's out of file give it a nudge 
        return new Position(line, EOF ? column + 1 : column, (int)index);
    }

    /// <summary>
    /// The IsUpcoming function checks if a given sequence matches the upcoming characters in a string
    /// </summary>
    /// <param name="sequence">A string representing a sequence of characters</param>
    /// <returns>
    /// The method is returning a boolean value
    /// </returns>
    private bool IsUpcoming(string sequence)
    {
        for (int i = 0; i < sequence.Length; i++)
        {
            if (!(sequence[i] == Peek(i)))
            {
                return false;
            }
        }

        Index += sequence.Length - 1;
        return true;
    }

    /// <summary>
    /// The function `AreUpcoming` checks if any of the given sequences are upcoming
    /// using the `IsUpcoming` method
    /// </summary>
    /// <param name="sequences">A string array containing sequences of characters to check</param>
    /// <returns>
    /// The method `AreUpcoming` returns a boolean value
    /// </returns>
    private bool AreUpcoming(params string[] sequences)
    {
        foreach (var seq in sequences)
        {
            if (IsUpcoming(seq))
                return true;
        }

        return false;
    }
}



// Provides methods and definitions for checking various characters and symbols
static class Checker
{
    // Some checks definitions
    public static char[] Separators  = {',','.',';',':','?'};
    public static char[] Operators   = {'=','+','-','*','/','%','!','&','|','^'};

    public static char[] OpnQuotes   = {'"','«','„','“'};
    public static char[] ClsQuotes   = {'"','»','“','”'};

    public static string[] Keywords = {"if","else","while","import"};
    public static string[] BigOprts = {"==","!=","++","--","??=","??","&&","||","^^","**"};


    public static char GetOtherPair(char C)
    {
        // Quotation marks
        if (OpnQuotes.Contains(C))
            return ClsQuotes[Array.IndexOf(OpnQuotes, C)];
        if (ClsQuotes.Contains(C))
            return OpnQuotes[Array.IndexOf(ClsQuotes, C)];

        throw new Exception($"Char \"{C}\" seems to not having a pair.");
    }
}