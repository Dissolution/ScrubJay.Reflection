namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Expects an instance of the type to be initialized on the stack.
    ///
    /// Initializes all the fields on a value type to null or an appropriate zero value.
    /// </summary>
    public Emit InitializeObject<TValueType>()
    {
        _innerEmit.InitializeObject<TValueType>();
        return this;
    }

    /// <summary>
    /// Expects an instance of the type to be initialized on the stack.
    ///
    /// Initializes all the fields on a value type to null or an appropriate zero value.
    /// </summary>
    public Emit InitializeObject(Type valueType)
    {
        _innerEmit.InitializeObject(valueType);
        return this;
    }
}
