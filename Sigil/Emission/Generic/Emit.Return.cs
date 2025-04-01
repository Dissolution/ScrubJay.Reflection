using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Ends the execution of the current method.
    ///
    /// If the current method does not return void, pops a value from the stack and returns it to the calling method.
    ///
    /// Return should leave the stack empty.
    /// </summary>
    public Emit<TDelegateType> Return()
    {
        if (_returnType == TypeOnStack.Get(typeof(void)))
        {
            UpdateState(Wrap(new[] { new StackTransition(0) }, "Return"));

            UpdateState(OpCodes.Ret, Wrap(StackTransition.None(), "Return"));

            _returns.Add(_il.Index);
            _mustMark = true;

            return this;
        }

        UpdateState(OpCodes.Ret, Wrap(StackTransition.Pop(_returnType), "Return"));

        _returns.Add(_il.Index);

        UpdateState(Wrap(new[] { new StackTransition(0) }, "Return"));
        _mustMark = true;

        var verify = _currentVerifiers.Return();
        if (!verify.Success)
        {
            throw new SigilVerificationException("Return", verify, _il.Instructions(_allLocals));
        }

        return this;
    }
}
