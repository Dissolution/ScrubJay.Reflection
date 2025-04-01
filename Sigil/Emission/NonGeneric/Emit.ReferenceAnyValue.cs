namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{

    /// <summary>
    /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
    ///
    /// __makeref(int) on the stack would become an int&amp;, for example.
    /// </summary>
    public ScrubJay.Sigil.Emission.NonGeneric.Emit ReferenceAnyValue<T>()
    {
        _innerEmit.ReferenceAnyValue<T>();
        return this;
    }

    /// <summary>
    /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
    ///
    /// __makeref(int) on the stack would become an int&amp;, for example.
    /// </summary>
    public ScrubJay.Sigil.Emission.NonGeneric.Emit ReferenceAnyValue(Type type)
    {
        _innerEmit.ReferenceAnyValue(type);
        return this;
    }
}
