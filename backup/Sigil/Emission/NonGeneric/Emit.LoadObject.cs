namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
    ///
    /// For primitive and reference types, use LoadIndirect().
    /// </summary>
    public Emit LoadObject<TValueType>(bool isVolatile = false, int? unaligned = null)
        where TValueType : struct
    {
        _innerEmit.LoadObject<TValueType>(isVolatile, unaligned);
        return this;
    }

    /// <summary>
    /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
    ///
    /// For primitive and reference types, use LoadIndirect().
    /// </summary>
    public Emit LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadObject(valueType, isVolatile, unaligned);
        return this;
    }
}
