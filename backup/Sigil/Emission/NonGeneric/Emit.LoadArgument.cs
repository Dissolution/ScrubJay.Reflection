
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Loads the argument at the given index (starting at 0) for the current method onto the stack.
    /// </summary>
    public Emit LoadArgument(ushort index)
    {
        _innerEmit.LoadArgument(index);
        return this;
    }
}