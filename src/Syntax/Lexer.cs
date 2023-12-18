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

        Token FabricateToken(TokenKind kind, bool sync = true)
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
            return FabricateToken(TokenKind.NewLine);

        if (char.IsWhiteSpace(Current))
        {
            while (Peek() == Current && char.IsWhiteSpace(Peek()))
                AddValue(-1);

            return FabricateToken(value.Length > 1 ? TokenKind.BigWhiteSpace : TokenKind.WhiteSpace);
        }

        // Operators etc
        switch (Current)
        {
            case char when IsUpcoming("+="):
                return FabricateToken(TokenKind.PlusEqual);
            case char when IsUpcoming("-="):
                return FabricateToken(TokenKind.MinusEqual);
            case char when IsUpcoming("*="):
                return FabricateToken(TokenKind.AsteriskEqual);
            case char when IsUpcoming("/="):
                return FabricateToken(TokenKind.ForwardSlashEqual);
            case char when IsUpcoming("%="):
                return FabricateToken(TokenKind.PercentEqual);
            case char when IsUpcoming("&="):
                return FabricateToken(TokenKind.ANDEqual);
            case char when IsUpcoming("|="):
                return FabricateToken(TokenKind.OREqual);
            case char when IsUpcoming("^="):
                return FabricateToken(TokenKind.XOREqual);
            case char when IsUpcoming("**="):
                return FabricateToken(TokenKind.PowerEqual);
            case char when IsUpcoming("&&="):
                return FabricateToken(TokenKind.LogicalANDEqual);
            case char when IsUpcoming("||="):
                return FabricateToken(TokenKind.LogicalOREqual);
            case char when IsUpcoming("??="):
                return FabricateToken(TokenKind.NullishCoalescingEqual);

            case char when IsUpcoming("=="):
                return FabricateToken(TokenKind.EqualEqual);
            case char when IsUpcoming("!="):
                return FabricateToken(TokenKind.NotEqual);
            case char when IsUpcoming("<="):
                return FabricateToken(TokenKind.LessEqual);
            case char when IsUpcoming(">="):
                return FabricateToken(TokenKind.GreaterEqual);

            case char when IsUpcoming("**"):
                return FabricateToken(TokenKind.Power);
            case char when IsUpcoming("++"):
                return FabricateToken(TokenKind.Increment);
            case char when IsUpcoming("--"):
                return FabricateToken(TokenKind.Decrement);
            case char when IsUpcoming("&&"):
                return FabricateToken(TokenKind.LogicalAND);
            case char when IsUpcoming("||"):
                return FabricateToken(TokenKind.LogicalOR);
            case char when IsUpcoming("??"):
                return FabricateToken(TokenKind.NullishCoalescing);

            case char when IsUpcoming("->"):
                return FabricateToken(TokenKind.RightArrow);

            case '=':
                return FabricateToken(TokenKind.Equal);
            case '+':
                return FabricateToken(TokenKind.Plus);
            case '-':
                return FabricateToken(TokenKind.Minus);
            case '*':
                return FabricateToken(TokenKind.Asterisk);
            case '/':
                return FabricateToken(TokenKind.ForwardSlash);
            case '%':
                return FabricateToken(TokenKind.Percent);
            case '!':
                return FabricateToken(TokenKind.BangMark);
            case '~':
                return FabricateToken(TokenKind.Tilde);
            case '&':
                return FabricateToken(TokenKind.Ampersand);
            case '|':
                return FabricateToken(TokenKind.Pipe);
            case '^':
                return FabricateToken(TokenKind.Caret);

            case '<':
                return FabricateToken(TokenKind.Less);
            case '>':
                return FabricateToken(TokenKind.Greater);

            case '.' when !char.IsDigit(Peek()):
                return FabricateToken(TokenKind.Dot);
            case ',':
                return FabricateToken(TokenKind.Comma);
            case ':':
                return FabricateToken(TokenKind.Colon);
            case ';':
                return FabricateToken(TokenKind.Semicolon);
            case '$':
                return FabricateToken(TokenKind.DollarSign);
            case '#':
                return FabricateToken(TokenKind.Hash);
            case '?':
                return FabricateToken(TokenKind.QuestionMark);

            case '(':
                return FabricateToken(TokenKind.OpenParenthesis);
            case '[':
                return FabricateToken(TokenKind.OpenSquareBracket);
            case '{':
                return FabricateToken(TokenKind.OpenCurlyBracket);
            case ')':
                return FabricateToken(TokenKind.CloseParenthesis);
            case ']':
                return FabricateToken(TokenKind.CloseSquareBracket);
            case '}':
                return FabricateToken(TokenKind.CloseCurlyBracket);
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
            return FabricateToken(type);
        }

        // Identifiers
        if (char.IsLetter(Current) || Current == '_')
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
                AddValue(-1);

            return FabricateToken(Checker.GetIdentifierKind(value));
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
                    return FabricateToken(TokenKind.Unknown, false);
                }

                if (Current == '\\')
                    Index++;
                Index++;
            }

            return FabricateToken(TokenKind.Char);
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
                    return FabricateToken(TokenKind.Unknown, false);
                }

                if (Current == '\\')
                    Index++;
                Index++;
            }

            return FabricateToken(TokenKind.String);
        }

        if (Checker.Separators.Contains(Current))
            return FabricateToken(TokenKind.Separator);

        Diagnostics.Report.UnrecognizedChar(Current, span);
        return FabricateToken(TokenKind.Unknown);
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
