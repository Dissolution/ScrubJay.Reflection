using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
#region Generic NewObject Constructor Finder Helpers

    /// <summary>
    /// Invokes the parameterless constructor of the given type, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType>()
    {
        return NewObject(typeof(TReferenceType));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14), typeof(TParameterType15));
    }

    /// <summary>
    /// Pops # of parameter arguments from the stack, invokes the the constructor of the given reference type that matches the given parameter types, and pushes a reference to the new object onto the stack.
    /// </summary>

    public Emit<TDelegateType> NewObject<TReferenceType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>()
    {
        return NewObject(typeof(TReferenceType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14), typeof(TParameterType15), typeof(TParameterType16));
    }

#endregion

    /// <summary>
    /// Pops parameterTypes.Length arguments from the stack, invokes the constructor on the given type that matches parameterTypes, and pushes a reference to the new object onto the stack.
    /// </summary>
    public Emit<TDelegateType> NewObject(Type type, params Type[] parameterTypes)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        if (parameterTypes == null)
        {
            throw new ArgumentNullException("parameterTypes");
        }

        var allCons = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
#if !NETSTANDARD
            | BindingFlags.CreateInstance
#endif
        );
        var cons = allCons.Where(
                    c =>
                        c.GetParameters().Length == parameterTypes.Length &&
                        c.GetParameters().Select((p, i) => p.ParameterType == parameterTypes[i]).Aggregate(true, (a, b) => a && b)
                ).SingleOrDefault();

        if (cons == null)
        {
            throw new InvalidOperationException("Type " + type + " must have a constructor that matches parameters [" + BufferedILGenerator<TDelegateType>.Join(", ", (parameterTypes)) + "]");
        }

        return NewObject(cons);
    }

    /// <summary>
    /// Pops # of parameters to the given constructor arguments from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
    /// </summary>
    public Emit<TDelegateType> NewObject(ConstructorInfo constructor)
    {
        if (constructor == null)
        {
            throw new ArgumentNullException("constructor");
        }

        var pts = (constructor.GetParameters()).Select(p => p.ParameterType).ToArray();

        return InnerNewObject(constructor, pts);
    }

    /// <summary>
    /// Pops # of parameters from the stack, invokes the constructor, and pushes a reference to the new object onto the stack.
    ///
    /// This method is provided as ConstructorBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
    /// do not match the given constructor, the produced code will be invalid.
    /// </summary>
    public Emit<TDelegateType> NewObject(ConstructorBuilder constructor, Type[] parameterTypes)
    {
        if(constructor == null)
        {
            throw new ArgumentNullException("constructor");
        }

        if (parameterTypes == null)
        {
            throw new ArgumentNullException("parameterTypes");
        }

        return InnerNewObject(constructor, parameterTypes);
    }

    Emit<TDelegateType> InnerNewObject(ConstructorInfo constructor, Type[] parameterTypes)
    {
        var expectedParams = (parameterTypes).Select(p => TypeOnStack.Get(p)).Reverse().ToList();

        var makesType = TypeOnStack.Get(constructor.DeclaringType);

        var transitions =
            new[]
            {
                new StackTransition(expectedParams, new [] { makesType }),
            };

        UpdateState(OpCodes.Newobj, constructor, Wrap(transitions, "NewObject"));

        return this;
    }
}
