namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
    /// </summary>
    public Emit StoreElement<TElementType>()
    {
        _innerEmit.StoreElement<TElementType>();
        return this;
    }

    /// <summary>
    /// Pops a value, an index, and a reference to an array off the stack.  Places the given value into the given array at the given index.
    /// </summary>
    public Emit StoreElement(Type elementType)
    {
        _innerEmit.StoreElement(elementType);
        return this;
    }
}
