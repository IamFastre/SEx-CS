using ErrorHandler;
using Util;

namespace Lexing;

// An enumeration of the token types possible in the source code
public enum TokenType
{
    Bad,
    WhiteSpace,
    Integer,
    Float,
    String,
    Identifier,
    Operator,
    EOF
}



// The Token class represents a token in a programming language,
// storing its value, type, and position in the source code
public class Token
{
    public string Value;
    public TokenType Type;
    public Span? Span;

    public Token(string value, TokenType type, Span? span)
    {
        Value = value;
        Type  = type;
        Span  = span;
    }

     public override string ToString()
     {
        TokenType[] noVal = {TokenType.WhiteSpace, TokenType.EOF};

        var typ = $"{Type}";
        var val = !noVal.Contains(Type) ? $": {Value}" : "";
        var pos = Span != null ? $" >> {Span}" : "";

        return $"[{typ}{val}]{pos}";
     }
}



// A class responsible for tokenizing a given text into a list of tokens
// based on specific rules and patterns
public class Lexer
{
    private int index;
    public readonly string Text;
    public List<Token> Tokens;

    private char Peek(int i = 1) => index+i < Text.Length ? Text[index+i] : '\0';

    private bool EOF
    {
        get { return index >= Text.Length; }
    }

    private char Current
    {
        get
        {
            return Peek(0);
        }
    }

    public Lexer(string text)
    {
        Text = text;
        Tokens = new();
    }


    /// <summary>
    /// The Start function is a method in C# that is used to initialize and start a program or
    /// application.
    /// </summary>
    public void Start()
    {
        while (true)
        {
            var token = GetToken();
            Tokens.Add(token);

            if (token.Type == TokenType.EOF)
                break;

            index++;
        }
    }


    public Token GetToken()
    {
        // Get the starting value and position of the token
        var value = Current.ToString();
        var span = new Span(GetPosition());

        // It adds to the value, very straight forward
        // The "advance" parameter however determines when to increment
        // that's if to increment at all
        // -1 => Before adding to value
        //  0 => Never
        //  1 => After adding to value
        void AddValue(int advance = 0)
        {
            if (advance == -1) index++;

            value += Current.ToString();
            span.End = GetPosition();

            if (advance == 1) index++;
        }

        // If it's a zero terminator return EOF token and advance
        if (Current == '\0')
            return new Token(value, TokenType.EOF, span);

        // If it's a whitespace, just add a whitespace token and advance
        if (char.IsWhiteSpace(Current))
            return new Token(value, TokenType.WhiteSpace, span);

        // If it's an operator, add an operator token and advance
        if (Checker.Operators.Contains(Current))
            return new Token(value, TokenType.Operator, span);

        // If it's a digit or a dot
        // loop until you reach its end and add it as a token and advance
        if (char.IsDigit(Current) || Current == '.')
        {
            int dots() => value.Count(s => s == '.');

            while (char.IsDigit(Peek()) || Peek() == '.' && !(dots() > 0))
            {
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
            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                AddValue(1);
            }
            return new Token(value, TokenType.Identifier, span);
        }

        // If it's an opening quotation mark
        // loop until you reach its end and add it as a token and advance
        if (Checker.OpnQuotes.Contains(Current))
        {
            // Get the closing quote for that opening one
            char clsQuote = Checker.ClsQuotes[Checker.GetQuoteIndex(Current)];
            index++;

            while (Current != clsQuote)
            {
                if (EOF || Current == '\n')
                {
                    Console.WriteLine("<!!> String Error");
                    return new Token(value, TokenType.Bad, span);
                }

                AddValue(1);
            }

            AddValue();
            return new Token(value, TokenType.String, span);
        }

        // If no type is met, it's bad
        return new Token(value, TokenType.Bad, span);
    }


    private Position GetPosition(int? index = null)
    {
        // If no specific index is given use instance's index
        // and if out of file boundaries use last index
        index ??= !EOF ? this.index : Text.Length - 1;
        // Line and Column start at one
        int line = 1;
        int column = 1;

        // Loop checking if you hit a new line character
        // if so add one to the line and start over with column
        for (int i = 0; i < index; i++)            
        {
            char character = Text[i];

            if (character == '\n')
            {
                line++;
                column = 0;
            }
            else
            {
                column++;
            }
        }

        // Return the position and if it's out of file give it a nudge 
        return new Position(line, EOF ? column + 1 : column);
    }
}



// Provides methods and definitions for checking various characters and symbols
static class Checker
{
    // Some checks definitions
    public static char[] Operators = {'=','+','-','*','/','%'};
    public static char[] OpnQuotes = {'"','«','„','“'};
    public static char[] ClsQuotes = {'"','»','“','”'};

    // Return its index to decide which is its closing quotation mark
    public static int GetQuoteIndex(char C)
    {
        if (OpnQuotes.Contains(C))
            return Array.IndexOf(OpnQuotes, C);
        else
            return Array.IndexOf(ClsQuotes, C);
    }
}