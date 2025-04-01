namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pushes a copy of the current top value on the stack.
    /// </summary>
    public Emit<TDelegateType> Duplicate()
    {
        UpdateState(OpCodes.Dup, Wrap(new [] { new StackTransition(isDuplicate: true) }, "Duplicate"));

        return this;
    }
}