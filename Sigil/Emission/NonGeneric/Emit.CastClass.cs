namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Cast a reference on the stack to the given reference type.
    ///
    /// If the cast is not legal, a CastClassException will be thrown at runtime.
    /// </summary>
    public Emit CastClass<TReferenceType>()
        where TReferenceType : class
    {
        _innerEmit.CastClass<TReferenceType>();
        return this;
    }

    /// <summary>
    /// Cast a reference on the stack to the given reference type.
    ///
    /// If the cast is not legal, a CastClassException will be thrown at runtime.
    /// </summary>
    public Emit CastClass(Type referenceType)
    {
        _innerEmit.CastClass(referenceType);
        return this;
    }
}
