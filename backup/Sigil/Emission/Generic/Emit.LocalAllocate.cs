namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a size from the stack, allocates size bytes on the local dynamic memory pool, and pushes a pointer to the allocated block.
    ///
    /// LocalAllocate can only be called if the stack is empty aside from the size value.
    ///
    /// Memory allocated with LocalAllocate is released when the current method ends execution.
    /// </summary>
    public Emit<TDelegateType> LocalAllocate()
    {
        if (_catchBlocks.Any(c => c.Value.Item2 == -1))
        {
            throw new InvalidOperationException("LocalAllocate cannot be used in a catch block");
        }

        if (_finallyBlocks.Any(f => f.Value.Item2 == -1))
        {
            throw new InvalidOperationException("LocalAllocate cannot be used in a finally block");
        }

        if (!AllowsUnverifiableCIL)
        {
            FailUnverifiable("LocalAllocate");
        }

        UpdateState(Wrap(new[] { new StackTransition(1) }, "LocalAllocate"));

        var transitions =
            new[] {
                new StackTransition(new [] { typeof(int) }, new [] { typeof(NativeIntType) }),
                new StackTransition(new [] { typeof(NativeIntType) }, new [] { typeof(NativeIntType) }),
            };

        UpdateState(OpCodes.Localloc, Wrap(transitions, "LocalAllocate"));

        return this;
    }
}
