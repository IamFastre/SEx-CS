using SEx.Generic.Constants;

namespace SEx.Scoping;

internal class TypeSymbol : Symbol
{
    public override SymbolKind Kind => SymbolKind.Type;
    protected TypeSymbol(string name) : base(name) { }

    public override int  GetHashCode()       => ToString().GetHashCode();
    public override bool Equals(object? obj) => obj?.ToString() == ToString();

    // Special types
    public static readonly TypeSymbol Unknown = new(CONSTS.UNKNOWN);
    public static readonly TypeSymbol Void    = new(CONSTS.VOID);
    public static readonly TypeSymbol Any     = new(CONSTS.ANY);

    // Data types
    public static readonly TypeSymbol Null    = new(CONSTS.NULL);
    public static readonly TypeSymbol Bool    = new(CONSTS.BOOLEAN);
    public static readonly TypeSymbol Number  = new(CONSTS.NUMBER);
    public static readonly TypeSymbol Int     = new(CONSTS.INTEGER);
    public static readonly TypeSymbol Float   = new(CONSTS.FLOAT);
    public static readonly TypeSymbol Char    = new(CONSTS.CHAR);
    public static readonly TypeSymbol String  = new(CONSTS.STRING);
    public static readonly TypeSymbol Range   = new(CONSTS.RANGE);
}
