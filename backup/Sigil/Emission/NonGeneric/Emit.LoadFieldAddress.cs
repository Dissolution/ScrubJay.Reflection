namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Loads the address of the given field onto the stack.
    /// 
    /// If the field is an instance field, a `this` reference is expected on the stack and will be popped.
    /// </summary>
    public Emit LoadFieldAddress(FieldInfo field)
    {
        _innerEmit.LoadFieldAddress(field);
        return this;
    }
}