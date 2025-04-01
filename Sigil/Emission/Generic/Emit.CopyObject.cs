namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
    ///
    /// Copies the given value type from the source to the destination.
    /// </summary>
    public Emit<TDelegateType> CopyObject<TValueType>()
        where TValueType : struct
    {
        return CopyObject(typeof(TValueType));
    }

    /// <summary>
    /// Takes a destination pointer, a source pointer as arguments.  Pops both off the stack.
    ///
    /// Copies the given value type from the source to the destination.
    /// </summary>
    public Emit<TDelegateType> CopyObject(Type valueType)
    {
        if (valueType == null)
        {
            throw new ArgumentNullException("valueType");
        }

        if (!valueType.IsValueType)
        {
            throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, []),
                new StackTransition(new [] { valueType.MakePointerType(), typeof(NativeIntType) }, []),
                new StackTransition(new [] { valueType.MakeByRefType(), typeof(NativeIntType) }, []),
                new StackTransition(new [] { typeof(NativeIntType), valueType.MakePointerType() }, []),
                new StackTransition(new [] { valueType.MakePointerType(), valueType.MakePointerType() }, []),
                new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakePointerType() }, []),
                new StackTransition(new [] { typeof(NativeIntType), valueType.MakeByRefType() }, []),
                new StackTransition(new [] { valueType.MakePointerType(), valueType.MakeByRefType() }, []),
                new StackTransition(new [] { valueType.MakeByRefType(), valueType.MakeByRefType() }, []),
            };

        UpdateState(OpCodes.Cpobj, valueType, Wrap(transitions, "CopyObject"));

        return this;
    }
}
