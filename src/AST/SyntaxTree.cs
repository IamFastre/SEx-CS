using SEx.Lex;
using SEx.Diagnose;

namespace SEx.AST;

public class SyntaxTree
{
    public Statement? Root { get; }
    public Diagnostics Diagnostics { get; }
    public Token EOF { get; }

    public SyntaxTree(Statement? root, Diagnostics diagnostics, Token eof)
    {
        Root = root;
        Diagnostics = diagnostics;
        EOF = eof;
    }

    public override string ToString() => $"{Root}";
}
