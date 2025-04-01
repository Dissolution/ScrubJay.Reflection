namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Expects an instance of the type to be initialized on the stack.
    ///
    /// Initializes all the fields on a value type to null or an appropriate zero value.
    /// </summary>
    public Emit<TDelegateType> InitializeObject<TValueType>()
    {
        return InitializeObject(typeof(TValueType));
    }

    /// <summary>
    /// Expects an instance of the type to be initialized on the stack.
    ///
    /// Initializes all the fields on a value type to null or an appropriate zero value.
    /// </summary>
    public Emit<TDelegateType> InitializeObject(Type valueType)
    {
        if (valueType == null)
        {
            throw new ArgumentNullException("valueType");
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(NativeIntType) }, []),
                new StackTransition(new [] { valueType.MakePointerType() }, []),
                new StackTransition(new [] { valueType.MakeByRefType() }, []),
            };

        UpdateState(OpCodes.Initobj, valueType, Wrap(transitions, "InitializeObject"));

        return this;
    }
}
