namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Boxes the given value type on the stack, converting it into a reference.
    /// </summary>
    public Emit Box<TValueType>()
        where TValueType : struct
    {
        _innerEmit.Box<TValueType>();
        return this;
    }

    /// <summary>
    /// Boxes the given value type on the stack, converting it into a reference.
    /// </summary>
    public Emit Box(Type valueType)
    {
        _innerEmit.Box(valueType);
        return this;
    }
}
