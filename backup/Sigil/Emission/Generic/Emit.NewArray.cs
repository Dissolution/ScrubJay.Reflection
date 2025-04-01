namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
    /// </summary>
    public Emit<TDelegateType> NewArray<TElementType>()
    {
        return NewArray(typeof(TElementType));
    }

    /// <summary>
    /// Pops a size from the stack, allocates a rank-1 array of the given type, and pushes a reference to the new array onto the stack.
    /// </summary>
    public Emit<TDelegateType> NewArray(Type elementType)
    {
        if (elementType == null)
        {
            throw new ArgumentNullException("elementType");
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(NativeIntType) }, new[] { elementType.MakeArrayType() }),
                new StackTransition(new [] { typeof(int) }, new[] { elementType.MakeArrayType() }),
            };

        UpdateState(OpCodes.Newarr, elementType, Wrap(transitions, "NewArray"));

        return this;
    }
}
