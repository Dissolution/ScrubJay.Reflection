namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
    ///
    /// This is analogous to C#'s `as` operator.
    /// </summary>
    public Emit IsInstance<T>()
    {
        _innerEmit.IsInstance<T>();
        return this;
    }

    /// <summary>
    /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
    ///
    /// This is analogous to C#'s `as` operator.
    /// </summary>
    public Emit IsInstance(Type type)
    {
        _innerEmit.IsInstance(type);
        return this;
    }
}
