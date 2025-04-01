namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value off the stack and throws it as an exception.
    ///
    /// Throw expects the value to be or extend from a System.Exception.
    /// </summary>
    public Emit<TDelegateType> Throw()
    {
        UpdateState(OpCodes.Throw, Wrap(StackTransition.Pop<Exception>(), "Throw"));
        UpdateState(Wrap(new[] { new StackTransition(new[] { typeof(PopAllType) }, []) }, "Throw"));

        _throws.Add(_il.Index);

        _mustMark = true;

        var verify = _currentVerifiers.Throw();
        if (!verify.Success)
        {
            throw new SigilVerificationException("Throw", verify, _il.Instructions(_allLocals));
        }

        return this;
    }
}
