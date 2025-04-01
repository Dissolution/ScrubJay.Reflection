namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Transfers control to another method.
    ///
    /// The parameters and calling convention of method must match the current one's.
    ///
    /// The stack must be empty to jump.
    ///
    /// Like the branching instructions, Jump cannot leave exception blocks.
    /// </summary>
    public Emit<TDelegateType> Jump(MethodInfo method)
    {
        if (method == null)
        {
            throw new ArgumentNullException("method");
        }

        if (method.CallingConvention != _callingConventions)
        {
            throw new ArgumentException("Jump expected a calling convention of " + _callingConventions + ", found " + method.CallingConvention);
        }

        var paras = method.GetParameters();

        if (paras.Length != _parameterTypes.Length)
        {
            throw new ArgumentException("Jump expected a method with " + _parameterTypes.Length + " parameters, found " + paras.Length);
        }

        if (!AllowsUnverifiableCIL)
        {
            FailUnverifiable("Jump");
        }

        if (_catchBlocks.Any(t => t.Value.Item2 == -1))
        {
            throw new InvalidOperationException("Jump cannot transfer control from a catch block");
        }

        if (_finallyBlocks.Any(t => t.Value.Item2 == -1))
        {
            throw new InvalidOperationException("Jump cannot transfer control from a finally block");
        }

        if (_tryBlocks.Any(t => t.Value.Item2 == -1))
        {
            throw new InvalidOperationException("Jump cannot transfer control from an exception block");
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "Jump"));

        for (var i = 0; i < paras.Length; i++)
        {
            var shouldBe = paras[i].ParameterType;
            var actuallyIs = _parameterTypes[i];

            if (!shouldBe.IsAssignableFrom(actuallyIs))
            {
                throw new SigilVerificationException("Jump expected the #" + i + " parameter to be assignable from " + actuallyIs + ", but found " + shouldBe, _il.Instructions(_allLocals));
            }
        }

        UpdateState(OpCodes.Jmp, method, [], Wrap(StackTransition.None(), "Jump"));

        return this;
    }
}
