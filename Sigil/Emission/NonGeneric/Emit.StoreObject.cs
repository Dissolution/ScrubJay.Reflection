namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
    ///
    /// For primitive and reference types use StoreIndirect.
    /// </summary>
    public Emit StoreObject<TValueType>(bool isVolatile = false, int? unaligned = null)
        where TValueType : struct
    {
        _innerEmit.StoreObject<TValueType>(isVolatile, unaligned);
        return this;
    }

    /// <summary>
    /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
    ///
    /// For primitive and reference types use StoreIndirect.
    /// </summary>
    public Emit StoreObject(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreObject(valueType, isVolatile, unaligned);
        return this;
    }
}
