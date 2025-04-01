namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
    /// </summary>
    public Emit<TDelegateType> LoadLength<TElementType>()
    {
        return LoadLength(typeof(TElementType));
    }

    /// <summary>
    /// Pops a reference to a rank 1 array off the stack, and pushes it's length onto the stack.
    /// </summary>
    public Emit<TDelegateType> LoadLength(Type elementType)
    {
        if (elementType == null)
        {
            throw new ArgumentNullException("elementType");
        }

        var transitions =
            new[] {
                new StackTransition(new [] { elementType.MakeArrayType() }, new [] { typeof(int) }),
            };

        UpdateState(OpCodes.Ldlen, Wrap(transitions, "LoadLength"));

        return this;
    }
}
