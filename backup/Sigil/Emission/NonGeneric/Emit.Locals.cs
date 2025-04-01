namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    /// </summary>
    public SigilLocal DeclareLocal<T>(string name = null)
    {
        return _innerEmit.DeclareLocal<T>(name);
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    /// </summary>
    public SigilLocal DeclareLocal(Type type, string name = null)
    {
        return _innerEmit.DeclareLocal(type, name);
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    /// </summary>
    public Emit DeclareLocal<T>(out SigilLocal sigilLocal, string name = null)
    {
        _innerEmit.DeclareLocal<T>(out sigilLocal, name);
        return this;
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    /// </summary>
    public Emit DeclareLocal(Type type, out SigilLocal sigilLocal, string name = null)
    {
        _innerEmit.DeclareLocal(type, out sigilLocal, name);
        return this;
    }
}
