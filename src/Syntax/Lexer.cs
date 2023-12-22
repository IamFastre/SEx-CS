using SEx.Diagnose;
using SEx.Generic.Text;

namespace SEx.Lex;
internal class Lexer
{
    public List<Token> Tokens      { get; }
    public Diagnostics Diagnostics { get; }

    private int Index;
    public readonly Source Source;

    private char Peek(int i = 1) => Index + i < Source.Length ? Source[Index + i] : '\0';
    private bool EOF             => Index >= Source.Length;
    private bool EOL             => Current == '\n';
    private char Current         => Peek(0);

    public Lexer(Source source, Diagnostics? diagnostics = null)
    {
        Source      = source;
        Diagnostics = diagnostics ?? new();
        Tokens      = new();
    }

    public Token[] Lex()
    {
        while (true)
        {
            var token = GetToken();
            Tokens.Add(token);

            if (token.Kind == TokenKind.EOF)
                break;

            Index++;
        }

        return Tokens.ToArray();
    }

    public Token GetToken()
    {
        var value = Current.ToString();
        var span  = new Span(GetPosition());

        Token CreateToken(TokenKind kind, bool sync = true)
            => new(sync ? SyncValue() : value, kind, span!);

        string AddValue(int advanceWhen = 0)
        {
            if (advanceWhen == -1) Index++;

            value += Current.ToString();
            span.End = GetPosition();

            if (advanceWhen == 1) Index++;

            return value;
        }

        string SyncValue()
        {
            var range = span.Start.Index..(Index+1);

            value    = Source[range];
            span.End = GetPosition();

            return value;
        }


        if (Current == '\0' || EOF)
            return new Token("\0", TokenKind.EOF, Source.GetLastPosition());

        // Whitespaces
        if (Current == '\n')
            return CreateToken(TokenKind.NewLine);

        if (char.IsWhiteSpace(Current))
        {
            while (Peek() == Current && char.IsWhiteSpace(Peek()))
                AddValue(-1);

            return CreateToken(value.Length > 1 ? TokenKind.BigWhiteSpace : TokenKind.WhiteSpace);
        }

        // Operators etc
        switch (Current)
        {
            case char when IsUpcoming("..."):
                return CreateToken(TokenKind.Ellipsis);
            case char when IsUpcoming(">>>"):
                return CreateToken(TokenKind.FunctionSymbol);

            case char when IsUpcoming("=>"):
                return CreateToken(TokenKind.EqualArrow);
            case char when IsUpcoming("->"):
                return CreateToken(TokenKind.DashArrow);

            case char when IsUpcoming("+="):
                return CreateToken(TokenKind.PlusEqual);
            case char when IsUpcoming("-="):
                return CreateToken(TokenKind.MinusEqual);
            case char when IsUpcoming("*="):
                return CreateToken(TokenKind.AsteriskEqual);
            case char when IsUpcoming("/="):
                return CreateToken(TokenKind.ForwardSlashEqual);
            case char when IsUpcoming("%="):
                return CreateToken(TokenKind.PercentEqual);
            case char when IsUpcoming("&="):
                return CreateToken(TokenKind.ANDEqual);
            case char when IsUpcoming("|="):
                return CreateToken(TokenKind.OREqual);
            case char when IsUpcoming("^="):
                return CreateToken(TokenKind.XOREqual);
            case char when IsUpcoming("**="):
                return CreateToken(TokenKind.PowerEqual);
            case char when IsUpcoming("&&="):
                return CreateToken(TokenKind.LogicalANDEqual);
            case char when IsUpcoming("||="):
                return CreateToken(TokenKind.LogicalOREqual);
            case char when IsUpcoming("??="):
                return CreateToken(TokenKind.NullishCoalescingEqual);

            case char when IsUpcoming("=="):
                return CreateToken(TokenKind.EqualEqual);
            case char when IsUpcoming("!="):
                return CreateToken(TokenKind.NotEqual);
            case char when IsUpcoming("<="):
                return CreateToken(TokenKind.LessEqual);
            case char when IsUpcoming(">="):
                return CreateToken(TokenKind.GreaterEqual);

            case char when IsUpcoming("**"):
                return CreateToken(TokenKind.Power);
            case char when IsUpcoming("++"):
                return CreateToken(TokenKind.Increment);
            case char when IsUpcoming("--"):
                return CreateToken(TokenKind.Decrement);
            case char when IsUpcoming("&&"):
                return CreateToken(TokenKind.LogicalAND);
            case char when IsUpcoming("||"):
                return CreateToken(TokenKind.LogicalOR);
            case char when IsUpcoming("??"):
                return CreateToken(TokenKind.NullishCoalescing);

            case '=':
                return CreateToken(TokenKind.Equal);
            case '+':
                return CreateToken(TokenKind.Plus);
            case '-':
                return CreateToken(TokenKind.Minus);
            case '*':
                return CreateToken(TokenKind.Asterisk);
            case '/':
                return CreateToken(TokenKind.ForwardSlash);
            case '%':
                return CreateToken(TokenKind.Percent);
            case '!':
                return CreateToken(TokenKind.BangMark);
            case '~':
                return CreateToken(TokenKind.Tilde);
            case '&':
                return CreateToken(TokenKind.Ampersand);
            case '|':
                return CreateToken(TokenKind.Pipe);
            case '^':
                return CreateToken(TokenKind.Caret);

            case '<':
                return CreateToken(TokenKind.Less);
            case '>':
                return CreateToken(TokenKind.Greater);

            case '.' when !char.IsDigit(Peek()):
                return CreateToken(TokenKind.Dot);
            case ',':
                return CreateToken(TokenKind.Comma);
            case ':':
                return CreateToken(TokenKind.Colon);
            case ';':
                return CreateToken(TokenKind.Semicolon);
            case '$':
                return CreateToken(TokenKind.DollarSign);
            case '#':
                return CreateToken(TokenKind.Hash);
            case '?':
                return CreateToken(TokenKind.QuestionMark);

            case '(':
                return CreateToken(TokenKind.OpenParenthesis);
            case '[':
                return CreateToken(TokenKind.OpenSquareBracket);
            case '{':
                return CreateToken(TokenKind.OpenCurlyBracket);
            case ')':
                return CreateToken(TokenKind.CloseParenthesis);
            case ']':
                return CreateToken(TokenKind.CloseSquareBracket);
            case '}':
                return CreateToken(TokenKind.CloseCurlyBracket);
        }

        // Numbers
        if (char.IsAsciiDigit(Current) || (Current == '.' && char.IsAsciiDigit(Peek())))
        {
            int dots() => value.Count(s => s == '.');

            while (char.IsAsciiDigit(Peek()) || Peek() == '.' && !(dots() > 0))
            {
                if (Peek() == '.' && !char.IsAsciiDigit(Peek(2)))
                    break;
                AddValue(-1);
            }

            if ("fF".Contains(Peek()))
                AddValue(-1);

            var type =  dots() == 1 || "fF".Contains(value[^1]) ? TokenKind.Float : TokenKind.Integer;
            return CreateToken(type);
        }

        // Identifiers
        if (char.IsLetter(Current) || Current == '_')
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
                AddValue(-1);

            return CreateToken(Checker.GetIdentifierKind(value));
        }

        // Characters
        if (Checker.OpnSQuotes.Contains(Current))
        {

            char clsQuote = Checker.GetOtherPair(Current);
            Index++;

            while (Current != clsQuote)
            {
                if (EOF || EOL)
                {
                    Diagnostics.Report.UnterminatedString(span);
                    return CreateToken(TokenKind.Unknown, false);
                }

                if (Current == '\\')
                    Index++;
                Index++;
            }

            return CreateToken(TokenKind.Char);
        }

        // Strings
        if (Checker.OpnDQuotes.Contains(Current))
        {

            char clsQuote = Checker.GetOtherPair(Current);
            Index++;

            while (Current != clsQuote)
            {
                if (EOF || EOL)
                {
                    Diagnostics.Report.UnterminatedString(span);
                    return CreateToken(TokenKind.Unknown, false);
                }

                if (Current == '\\')
                    Index++;
                Index++;
            }

            return CreateToken(TokenKind.String);
        }

        if (Checker.Separators.Contains(Current))
            return CreateToken(TokenKind.Separator);

        Diagnostics.Report.UnrecognizedChar(Current, span);
        return CreateToken(TokenKind.Unknown);
    }


    private Position GetPosition(int? index = null)
    {
        index ??= !EOF ? Index : Source.Length - 1;
        int line = 1;
        int column = 1;

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

        return new Position(line, EOF ? column + 1 : column, (int) index);
    }

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
