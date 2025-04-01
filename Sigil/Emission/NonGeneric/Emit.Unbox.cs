namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
    ///
    /// To load the value directly onto the stack, use UnboxAny().
    /// </summary>
    public Emit Unbox<TValueType>()
    {
        _innerEmit.Unbox<TValueType>();
        return this;
    }

    /// <summary>
    /// Pops a boxed value from the stack and pushes a pointer to it's unboxed value.
    ///
    /// To load the value directly onto the stack, use UnboxAny().
    /// </summary>
    public Emit Unbox(Type valueType)
    {
        _innerEmit.Unbox(valueType);
        return this;
    }

    /// <summary>
    /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
    ///
    /// To get an address for the unboxed value instead, use Unbox().
    /// </summary>
    public Emit UnboxAny<TValueType>()
    {
        _innerEmit.UnboxAny<TValueType>();
        return this;
    }

    /// <summary>
    /// Pops a boxed value from the stack, unboxes it and pushes the value onto the stack.
    ///
    /// To get an address for the unboxed value instead, use Unbox().
    /// </summary>
    public Emit UnboxAny(Type valueType)
    {
        _innerEmit.UnboxAny(valueType);
        return this;
    }
}
