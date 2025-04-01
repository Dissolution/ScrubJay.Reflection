namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
#region Generic NewObject Constructor Finder Helpers

    /// <summary>
    /// Invokes the parameterless constructor of the given type, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType>()
    {
        _innerEmit.NewObject<TReferenceType>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>();
        return this;
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>()
    {
        _innerEmit.NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>();
        return this;
    }

#endregion

    /// <summary>
    /// Pops parameterTypes.Length arguments from the stack, invokes the constructor on the given type that matches parameterTypes, and pushes a reference to the new object onto the stack.
    /// </summary>
    public Emit NewObject(Type type, params Type[] parameterTypes)
    {
        _innerEmit.NewObject(type, parameterTypes);
        return this;
    }

    /// <summary>
    /// Pops # of parameters to the given constructor arguments from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
    /// </summary>
    public Emit NewObject(ConstructorInfo constructor)
    {
        _innerEmit.NewObject(constructor);
        return this;
    }

    /// <summary>
    /// Pops # of parameters from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
    ///
    /// This method is provided as ConstructorBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
    /// do not match the given constructor, the produced code will be invalid.
    /// </summary>
    public Emit NewObject(ConstructorBuilder constructor, Type[] parameterTypes)
    {
        _innerEmit.NewObject(constructor, parameterTypes);
        return this;
    }
}
