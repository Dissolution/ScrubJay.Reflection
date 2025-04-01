namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Emits an instruction that does nothing.
    /// </summary>
    public Emit<TDelegateType> Nop()
    {
        UpdateState(OpCodes.Nop, Wrap(StackTransition.None(), "Nop"));

        return this;
    }
}