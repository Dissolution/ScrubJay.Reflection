
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Expects a destination pointer, a source pointer, and a length on the stack.  Pops all three values.
    /// 
    /// Copies length bytes from destination to the source.
    /// </summary>
    public Emit CopyBlock(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.CopyBlock(isVolatile, unaligned);
        return this;
    }
}