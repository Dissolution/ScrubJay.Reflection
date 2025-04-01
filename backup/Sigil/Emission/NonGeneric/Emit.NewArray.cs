namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
    /// </summary>
    public Emit NewArray<TElementType>()
    {
        _innerEmit.NewArray<TElementType>();
        return this;
    }

    /// <summary>
    /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
    /// </summary>
    public Emit NewArray(Type elementType)
    {
        _innerEmit.NewArray(elementType);
        return this;
    }
}
