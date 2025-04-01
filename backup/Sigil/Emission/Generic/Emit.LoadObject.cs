namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
    ///
    /// For primitive and reference types, use LoadIndirect().
    /// </summary>
    public Emit<TDelegateType> LoadObject<TValueType>(bool isVolatile = false, int? unaligned = null)
        where TValueType : struct
    {
        return LoadObject(typeof(TValueType), isVolatile, unaligned);
    }

    /// <summary>
    /// Pops a pointer from the stack, and pushes the given value type it points to onto the stack.
    ///
    /// For primitive and reference types, use LoadIndirect().
    /// </summary>
    public Emit<TDelegateType> LoadObject(Type valueType, bool isVolatile = false, int? unaligned = null)
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
            UpdateState(OpCodes.Volatile, Wrap(StackTransition.None(), "LoadObject"));
        }

        if (unaligned.HasValue)
        {
            UpdateState(OpCodes.Unaligned, (byte)unaligned.Value, Wrap(StackTransition.None(), "LoadObject"));
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(NativeIntType) }, new [] { valueType }),
                new StackTransition(new [] { valueType.MakePointerType() }, new [] { valueType }),
                new StackTransition(new [] { valueType.MakeByRefType() }, new [] { valueType }),
            };

        UpdateState(OpCodes.Ldobj, valueType, Wrap(transitions, "LoadObject"));

        return this;
    }
}
