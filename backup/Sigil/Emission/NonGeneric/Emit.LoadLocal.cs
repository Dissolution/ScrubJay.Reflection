
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Loads the value in the given local onto the stack.
    ///
    /// To create a local, use DeclareLocal().
    /// </summary>
    public Emit LoadLocal(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocal(sigilLocal);
        return this;
    }

    /// <summary>
    /// Loads the value in the local with the given name onto the stack.
    /// </summary>
    public Emit LoadLocal(string name)
    {
        _innerEmit.LoadLocal(name);
        return this;
    }
}
