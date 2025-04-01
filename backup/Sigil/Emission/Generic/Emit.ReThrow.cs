namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// From within a catch block, rethrows the exception that caused the catch block to be entered.
    /// </summary>
    public Emit<TDelegateType> ReThrow()
    {
        if(!_catchBlocks.Any(c => c.Value.Item2 == -1))
        {
            throw new InvalidOperationException("ReThrow is only legal in a catch block");
        }

        UpdateState(OpCodes.Rethrow, Wrap(StackTransition.None(), "ReThrow"));
        UpdateState(Wrap(new[] { new StackTransition(new[] { typeof(PopAllType) }, []) }, "ReThrow"));

        _throws.Add(_il.Index);

        _mustMark = true;

        var verify = _currentVerifiers.ReThrow();
        if (!verify.Success)
        {
            throw new SigilVerificationException("ReThrow", verify, _il.Instructions(_allLocals));
        }

        return this;
    }
}
