namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
    /// </summary>
    public Emit LoadLength<TElementType>()
    {
        _innerEmit.LoadLength<TElementType>();
        return this;
    }

    /// <summary>
    /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
    /// </summary>
    public Emit LoadLength(Type elementType)
    {
        _innerEmit.LoadLength(elementType);
        return this;
    }
}
