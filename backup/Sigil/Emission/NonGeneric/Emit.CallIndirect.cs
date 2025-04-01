namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes a void return and no parameters.
    /// </summary>

    public Emit CallIndirect(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect(callConventions);
        return this;
    }

#region Generic CallIndirect Finder Helpers

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and no parameters.
    /// </summary>

    public Emit CallIndirect<TReturn>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturn>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturn, T1>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturn, T1>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturn, T1, T2>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturn, T1, T2>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>(callConventions);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>(CallingConventions callConventions)
    {
        _innerEmit.CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>(callConventions);
        return this;
    }

#endregion

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This override allows an arglist to be passed for calling VarArgs methods.
    /// </summary>
    public Emit CallIndirect(CallingConventions callConventions, Type returnType, Type[] parameterTypes, Type[] arglist = null)
    {
        _innerEmit.CallIndirect(callConventions, returnType, parameterTypes, arglist);
        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    /// </summary>
    public Emit CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
    {
        _innerEmit.CallIndirect(callConventions, returnType, parameterTypes);
        return this;
    }
}
