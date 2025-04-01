
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Emits an instruction that does nothing.
    /// </summary>
    public Emit Nop()
    {
        _innerEmit.Nop();
        return this;
    }
}