namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Expects a reference to an array and an index on the stack.
    ///
    /// Pops both, and pushes the element in the array at the index onto the stack.
    /// </summary>
    public Emit LoadElement<TElementType>()
    {
        _innerEmit.LoadElement<TElementType>();
        return this;
    }

    /// <summary>
    /// Expects a reference to an array and an index on the stack.
    ///
    /// Pops both, and pushes the element in the array at the index onto the stack.
    /// </summary>
    public Emit LoadElement(Type elementType)
    {
        _innerEmit.LoadElement(elementType);
        return this;
    }
}
