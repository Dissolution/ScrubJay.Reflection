namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a value from the stack and stores it in the given field.
    /// 
    /// If the field is an instance member, both a value and a reference to the instance are popped from the stack.
    /// </summary>
    public Emit StoreField(FieldInfo field, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreField(field, isVolatile, unaligned);
        return this;
    }
}