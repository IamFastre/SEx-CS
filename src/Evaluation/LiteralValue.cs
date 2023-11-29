
namespace SEx.Evaluate.Values;


internal abstract class LiteralValue
{
    public abstract object Value { get; }
    public abstract ValType Type { get; }

    public abstract override string ToString();
}
