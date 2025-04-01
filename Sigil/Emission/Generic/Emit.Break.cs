namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Emits a break instruction for use with a debugger.
    /// </summary>
    public Emit<TDelegateType> Break()
    {
        UpdateState(OpCodes.Break, Wrap(StackTransition.None(), "Break"));

        return this;
    }
}