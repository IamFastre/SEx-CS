using SEx.AST;
using SEx.Generic.Constants;
using SEx.Generic.Text;
using SEx.Scoping.Symbols;

namespace SEx.SemanticAnalysis;

public abstract class SemanticNode
{
    public abstract SemanticKind Kind { get; }
    public abstract Span         Span { get; }

    public static TypeSymbol ToValueType(NodeKind kind) => kind switch
    {
        NodeKind.Unknown      => TypeSymbol.Unknown,
        NodeKind.Null         => TypeSymbol.Null,
        NodeKind.Boolean      => TypeSymbol.Boolean,
        NodeKind.Integer      => TypeSymbol.Integer,
        NodeKind.Float        => TypeSymbol.Float,
        NodeKind.Char         => TypeSymbol.Char,
        NodeKind.String       => TypeSymbol.String,
        NodeKind.FormatString => TypeSymbol.String,

        _ => throw new Exception("Unknown literal kind"),
    };

    public abstract IEnumerable<SemanticNode> GetChildren();

    public void PrintTree()
        => WriteTree(Console.Out, "");

    public void WriteTree(TextWriter writer, string indent = "")
    {
        string middle   = C.DIM + "├──" + C.END;
        string vertical = C.DIM + "│  " + C.END;
        string last     = C.DIM + "└──" + C.END;
        string space    = C.DIM + "   " + C.END;

        if (this is SemanticExpressionStatement es)
        {
            es.Expression.WriteTree(writer, indent);
            return;
        }

        var children  = GetChildren().ToArray();
        string header = this switch
        {
            SemanticLiteral              l => $"<{C.BLUE2}{l.Kind}({C.YELLOW2}{l.Value         }{C.BLUE2}):{C.GREEN2}{l.Type}{C.END}>",
            SemanticName                 l => $"<{C.BLUE2}{l.Kind}({C.YELLOW2}{l.Symbol.Name   }{C.BLUE2}):{C.GREEN2}{l.Type}{C.END}>",
            SemanticStringFragment       s => $"<{C.BLUE2}{s.Kind}({C.YELLOW2}\"{s.Value     }\"{C.BLUE2}){C.END}>",

            SemanticAssignment           a => $"<{C.BLUE2}{a.Kind}({C.YELLOW2}{a.Operator      }{C.BLUE2}):{C.GREEN2}{a.Type}{C.END}>",
            SemanticUnaryOperation       u => $"<{C.BLUE2}{u.Kind}({C.YELLOW2}{u.OperationKind }{C.BLUE2}):{C.GREEN2}{u.Type}{C.END}>",
            SemanticCountingOperation    c => $"<{C.BLUE2}{c.Kind}({C.YELLOW2}{c.OperationKind }{C.BLUE2}):{C.GREEN2}{c.Type}{C.END}>",
            SemanticBinaryOperation      b => $"<{C.BLUE2}{b.Kind}({C.YELLOW2}{b.Operator.Kind }{C.BLUE2}):{C.GREEN2}{b.Type}{C.END}>",
            SemanticConversionOperation c => $"<{C.BLUE2}{c.Kind}({C.YELLOW2}{c.ConversionKind}{C.BLUE2}):{C.GREEN2}{c.Type}{C.END}>",

            SemanticDeclarationStatement d => $"<{C.MAGENTA2}{d.Kind}({C.RED2}{(d.Variable.IsConstant ? "*" : "")}{C.YELLOW2}{d.Variable.Name}{C.MAGENTA2}:{C.GREEN2}{d.Variable.Type}{C.MAGENTA2}){C.END}>",
            SemanticFunctionStatement    f => $"<{C.MAGENTA2}{f.Kind}({C.YELLOW2}{f.Function}{C.MAGENTA2}){C.END}>",
            SemanticForStatement         f => $"<{C.MAGENTA2}{f.Kind}({C.RED2}*{C.YELLOW2}{f.Variable.Name}{C.MAGENTA2}:{C.GREEN2}{f.Variable.Type}{C.MAGENTA2}){C.END}>",

            SemanticFailedExpression     f => $"<{C.GREEN2}{f.Type}{C.END}>",
            SemanticExpression           e => $"<{C.BLUE2}{e.Kind}:{C.GREEN2}{e.Type}{C.END}>",
            SemanticClause               c => $"[{C.RED2}{c.Kind}{C.END}]",
            _                              => $"{{{C.MAGENTA2}{Kind}{C.END}}}",
        };
    
        writer.WriteLine(header);
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i];

            if (child == children.LastOrDefault())
                writer.Write(indent + last);
            else
                writer.Write(indent + middle);

            if (i == children.Length - 1)
                child.WriteTree(writer, indent + space);
            else
                child.WriteTree(writer, indent + vertical);
        }
    }
}

public abstract class SemanticClause     : SemanticNode { }

public abstract class SemanticStatement  : SemanticNode
{
    public bool HasScope { get; set; } = false;
}

public abstract class SemanticExpression : SemanticNode
{
    public TypeSymbol Type { get; private set; }

    protected SemanticExpression(TypeSymbol type)
        => Type = type;
}
