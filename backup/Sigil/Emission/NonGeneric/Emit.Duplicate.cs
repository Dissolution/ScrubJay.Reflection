
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pushes a copy of the current top value on the stack.
    /// </summary>
    public Emit Duplicate()
    {
        _innerEmit.Duplicate();
        return this;
    }
}