
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pushes a pointer to the given local onto the stack.
    ///
    /// To create a local, use DeclareLocal.
    /// </summary>
    public Emit LoadLocalAddress(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocalAddress(sigilLocal);
        return this;
    }

    /// <summary>
    /// Pushes a pointer to the local with the given name onto the stack.
    /// </summary>
    public Emit LoadLocalAddress(string name)
    {
        _innerEmit.LoadLocalAddress(name);
        return this;
    }
}
