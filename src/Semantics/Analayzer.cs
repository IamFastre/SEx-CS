
using SEx.AST;
using SEx.Diagnose;
using SEx.Lex;
using SEx.Parse;
using SEx.Symbols.Types;

namespace SEx.Semantics;

internal class Analyzer
{
    public Diagnostics Diagnostics { get; }
    public SyntaxTree Tree { get; }

    public Analyzer(Parser parser) : this(parser.Tree) {}
    public Analyzer(SyntaxTree tree)
    {
        Diagnostics = tree.Diagnostics;
        Tree = tree;
    }

    // public static SemanticTree Analyze(SyntaxTree tree)
    // {
        
    // }
}

internal class SemanticTree
{
    public SemanticStatement Root { get; }
    public Diagnostics Diagnostics { get; }
    public Token EOF { get; }

    public SemanticTree(SemanticStatement root, Diagnostics diagnostics, Token eof)
    {
        Root = root;
        Diagnostics = diagnostics;
        EOF = eof;
    }
}

public enum SemanticKind
{
    Literal,
    BinaryOperation
}

internal abstract class SemanticNode
{
    public abstract SemanticKind Kind { get; }
    public abstract VTypes Type { get; }
}

internal abstract class SemanticStatement : SemanticNode {}
internal abstract class SemanticExpression : SemanticStatement {}

internal abstract class SemanticLiteral : SemanticExpression
{
    public object Value { get; }

    public SemanticLiteral()
    {
        Value = Type switch
        {
            _ => 1,
        };
    }
}