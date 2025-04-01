namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Removes the top value on the stack.
    /// </summary>
    public Emit<TDelegateType> Pop()
    {
        UpdateState(OpCodes.Pop, Wrap(StackTransition.Pop<WildcardType>(), "Pop"));

        return this;
    }
}