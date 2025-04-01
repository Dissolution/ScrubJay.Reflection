namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{

    /// <summary>
    /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
    ///
    /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
    /// </summary>
    public Emit MakeReferenceAny<T>()
    {
        _innerEmit.MakeReferenceAny<T>();
        return this;
    }

    /// <summary>
    /// Converts a pointer or reference to a value on the stack into a TypedReference of the given type.
    ///
    /// TypedReferences can be used with ReferenceAnyType and ReferenceAnyValue to pass arbitrary types as parameters.
    /// </summary>
    public Emit MakeReferenceAny(Type type)
    {
        _innerEmit.MakeReferenceAny(type);
        return this;
    }
}
