namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
    ///
    /// Copies the given value type from the source to the destination.
    /// </summary>
    public Emit CopyObject<TValueType>()
        where TValueType : struct
    {
        _innerEmit.CopyObject<TValueType>();
        return this;
    }

    /// <summary>
    /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
    ///
    /// Copies the given value type from the source to the destination.
    /// </summary>
    public Emit CopyObject(Type valueType)
    {
        _innerEmit.CopyObject(valueType);
        return this;
    }
}
