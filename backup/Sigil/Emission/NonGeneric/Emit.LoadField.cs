namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Loads a field onto the stack.
    /// 
    /// Instance fields expect a reference on the stack, which is popped.
    /// </summary>
    public Emit LoadField(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
    {
        _innerEmit.LoadField(field, isVolatile, unaligned);
        return this;
    }
}