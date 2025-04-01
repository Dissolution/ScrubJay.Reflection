
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Removes the top value on the stack.
    /// </summary>
    public Emit Pop()
    {
        _innerEmit.Pop();
        return this;
    }
}