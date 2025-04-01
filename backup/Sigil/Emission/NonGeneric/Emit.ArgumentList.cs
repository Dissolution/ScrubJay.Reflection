
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pushes a pointer to the current argument list onto the stack.
    /// 
    /// This instruction can only be used in VarArgs methods.
    /// </summary>
    public Emit ArgumentList()
    {
        _innerEmit.ArgumentList();
        return this;
    }
}