namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
    /// </summary>
    public Emit LoadIndirect<T>(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadIndirect<T>(isVolatile, unaligned);
        return this;
    }

    /// <summary>
    /// Pops a pointer from the stack and pushes the value (of the given type) at that address onto the stack.
    /// </summary>
    public Emit LoadIndirect(Type type, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadIndirect(type, isVolatile, unaligned);
        return this;
    }
}
