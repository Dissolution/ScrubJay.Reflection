namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pushes the size of the given value type onto the stack.
    /// </summary>
    public Emit SizeOf<TValueType>()
        where TValueType : struct
    {
        _innerEmit.SizeOf<TValueType>();
        return this;
    }

    /// <summary>
    /// Pushes the size of the given value type onto the stack.
    /// </summary>
    public Emit SizeOf(Type valueType)
    {
        _innerEmit.SizeOf(valueType);
        return this;
    }
}
