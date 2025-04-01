namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pushes a pointer to the current argument list onto the stack.
    ///
    /// This instruction can only be used in VarArgs methods.
    /// </summary>
    public Emit<TDelegateType> ArgumentList()
    {
        if (!AllowsUnverifiableCIL)
        {
            FailUnverifiable("ArgumentList");
        }

        if (_callingConventions != System.Reflection.CallingConventions.VarArgs)
        {
            throw new InvalidOperationException("ArgumentList can only be called in VarArgs methods");
        }

        UpdateState(OpCodes.Arglist, Wrap(StackTransition.Push<NativeIntType>(), "ArgumentList"));

        return this;
    }
}
