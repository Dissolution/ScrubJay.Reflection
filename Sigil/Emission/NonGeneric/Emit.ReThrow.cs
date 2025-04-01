
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// From within a catch block, rethrows the exception that caused the catch block to be entered.
    /// </summary>
    public Emit ReThrow()
    {
        _innerEmit.ReThrow();
        return this;
    }
}