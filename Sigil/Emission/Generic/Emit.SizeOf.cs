namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pushes the size of the given value type onto the stack.
    /// </summary>
    public Emit<TDelegateType> SizeOf<TValueType>()
        where TValueType : struct
    {
        return SizeOf(typeof(TValueType));
    }

    /// <summary>
    /// Pushes the size of the given value type onto the stack.
    /// </summary>
    public Emit<TDelegateType> SizeOf(Type valueType)
    {
        if (valueType == null)
        {
            throw new ArgumentNullException("valueType");
        }

        if (!valueType.IsValueType)
        {
            throw new ArgumentException("valueType must be a ValueType");
        }

        UpdateState(OpCodes.Sizeof, valueType, Wrap(StackTransition.Push<int>(), "SizeOf"));

        return this;
    }
}
