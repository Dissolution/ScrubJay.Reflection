namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
    ///
    /// For primitive and reference types use StoreIndirect.
    /// </summary>
    public Emit<TDelegateType> StoreObject<TValueType>(bool isVolatile = false, int? unaligned = null)
        where TValueType : struct
    {
        return StoreObject(typeof(TValueType), isVolatile, unaligned);
    }

    /// <summary>
    /// Pops a value type and a pointer off of the stack and copies the given value to the given address.
    ///
    /// For primitive and reference types use StoreIndirect.
    /// </summary>
    public Emit<TDelegateType> StoreObject(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        if (valueType == null)
        {
            throw new ArgumentNullException("valueType");
        }

        if (!valueType.IsValueType)
        {
            throw new ArgumentException("valueType must be a ValueType");
        }

        if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
        {
            throw new ArgumentException("unaligned must be null, 1, 2, or 4");
        }

        if (isVolatile)
        {
            UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "StoreObject"));
        }

        if (unaligned.HasValue)
        {
            UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "StoreObject"));
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { valueType, typeof(NativeIntType) }, []),
                new StackTransition(new [] { valueType, valueType.MakePointerType() }, []),
                new StackTransition(new [] { valueType, valueType.MakeByRefType() }, []),
            };

        UpdateState(OpCodes.Stobj, valueType, Wrap(transitions, "StoreObject"));

        return this;
    }
}
