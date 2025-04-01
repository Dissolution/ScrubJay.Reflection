
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Emits a break instruction for use with a debugger.
    /// </summary>
    public Emit Break()
    {
        _innerEmit.Break();
        return this;
    }
}