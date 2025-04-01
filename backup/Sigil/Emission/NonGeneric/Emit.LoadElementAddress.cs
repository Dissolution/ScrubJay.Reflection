namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Expects a reference to an array of the given element type and an index on the stack.
    ///
    /// Pops both, and pushes the address of the element at the given index.
    /// </summary>
    public Emit LoadElementAddress<TElementType>()
    {
        _innerEmit.LoadElementAddress<TElementType>();
        return this;
    }

    /// <summary>
    /// Expects a reference to an array of the given element type and an index on the stack.
    ///
    /// Pops both, and pushes the address of the element at the given index.
    /// </summary>
    public Emit LoadElementAddress(Type elementType)
    {
        _innerEmit.LoadElementAddress(elementType);
        return this;
    }
}
