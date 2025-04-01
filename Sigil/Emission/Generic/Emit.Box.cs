namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Boxes the given value type on the stack, converting it into a reference.
    /// </summary>
    public Emit<TDelegateType> Box<TValueType>()
        where TValueType : struct
    {
        return Box(typeof(TValueType));
    }

    /// <summary>
    /// Boxes the given value type on the stack, converting it into a reference.
    /// </summary>
    public Emit<TDelegateType> Box(Type valueType)
    {
        if (valueType == null)
        {
            throw new ArgumentNullException("valueType");
        }

        if (!valueType.IsValueType || valueType == typeof(void))
        {
            throw new ArgumentException("Only ValueTypes can be boxed, found " + valueType, "valueType");
        }

        if (!AllowsUnverifiableCIL && valueType.IsByRef)
        {
            throw new InvalidOperationException("Box with by-ref types is not verifiable");
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { valueType }, new [] { typeof(object) }),
            };

        UpdateState(OpCodes.Box, valueType, Wrap(transitions, "Box"));

        return this;
    }
}
