using ScrubJay.Sigil.Extensions;
using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes a void return and no parameters.
    /// </summary>

    public Emit<TDelegateType> CallIndirect(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(void), []);
    }

#region Generic CallIndirect Finder Helpers

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and no parameters.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14), typeof(TParameterType15));
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
    /// </summary>

    public Emit<TDelegateType> CallIndirect<TReturnType, TParameterType1, TParameterType2, TParameterType3, TParameterType4, TParameterType5, TParameterType6, TParameterType7, TParameterType8, TParameterType9, TParameterType10, TParameterType11, TParameterType12, TParameterType13, TParameterType14, TParameterType15, TParameterType16>(CallingConventions callConventions)
    {
        return CallIndirect(callConventions, typeof(TReturnType), typeof(TParameterType1), typeof(TParameterType2), typeof(TParameterType3), typeof(TParameterType4), typeof(TParameterType5), typeof(TParameterType6), typeof(TParameterType7), typeof(TParameterType8), typeof(TParameterType9), typeof(TParameterType10), typeof(TParameterType11), typeof(TParameterType12), typeof(TParameterType13), typeof(TParameterType14), typeof(TParameterType15), typeof(TParameterType16));
    }

#endregion

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    ///
    /// This override allows an arglist to be passed for calling VarArgs methods.
    /// </summary>
    public Emit<TDelegateType> CallIndirect(CallingConventions callConventions, Type returnType, Type[] parameterTypes, Type[] arglist = null)
    {
        if (returnType == null)
        {
            throw new ArgumentNullException("returnType");
        }

        if (parameterTypes == null)
        {
            throw new ArgumentNullException("parameterTypes");
        }

        var known = CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs;
        known = ~known;

        if ((callConventions & known) != 0)
        {
            throw new ArgumentException("Unexpected value not in CallingConventions", "callConventions");
        }

        if (!AllowsUnverifiableCIL)
        {
            FailUnverifiable("CallIndirect");
        }

        if (HasFlag(callConventions, CallingConventions.VarArgs) && !HasFlag(callConventions, CallingConventions.Standard))
        {
            if (arglist == null)
            {
                throw new InvalidOperationException("When calling a VarArgs method, arglist must be set");
            }
        }

        var takeExtra = 1;

        if (HasFlag(callConventions, CallingConventions.HasThis))
        {
            takeExtra++;
        }

        IEnumerable<StackTransition> transitions;
        if (HasFlag(callConventions, CallingConventions.HasThis))
        {
            var p = new List<Type>();
            p.Add(typeof(NativeIntType));
            p.AddRange(parameterTypes.Reversed());
            p.Add(typeof(WildcardType));

            if (returnType != typeof(void))
            {
                transitions =
                    new[]
                    {
                        new StackTransition(p, new [] { returnType }),
                    };
            }
            else
            {
                transitions =
                    new[]
                    {
                        new StackTransition(p, []),
                    };
            }
        }
        else
        {
            var p = new List<Type>
            {
                typeof( NativeIntType ),
            };

            p.AddRange(parameterTypes.Reversed());

            if (returnType != typeof(void))
            {
                transitions =
                    new[]
                    {
                        new StackTransition(p, new [] { returnType }),
                    };
            }
            else
            {
                transitions =
                    new[]
                    {
                        new StackTransition(p, []),
                    };
            }
        }

        var onStack = _currentVerifiers.InferStack(transitions.ElementAt(0).PoppedFromStack.Length);
        if (onStack != null && onStack.Count > 0)
        {
            var funcPtr = onStack.First();

            if (funcPtr == TypeOnStack.Get<NativeIntType>() && funcPtr.HasAttachedMethodInfo)
            {
                if (funcPtr.CallingConvention != callConventions)
                {
                    throw new SigilVerificationException("CallIndirect expects method calling conventions to match, found " + funcPtr.CallingConvention + " on the stack", _il.Instructions(_allLocals));
                }

                if (HasFlag(callConventions, CallingConventions.HasThis))
                {
                    var thisRef = onStack.Last();

                    if (!ExtensionMethods.IsAssignableFrom(funcPtr.InstanceType, thisRef))
                    {
                        throw new SigilVerificationException("CallIndirect expects a 'this' value assignable to " + funcPtr.InstanceType + ", found " + thisRef, _il.Instructions(_allLocals));
                    }
                }

                if (funcPtr.ReturnType != returnType)
                {
                    throw new SigilVerificationException("CallIndirect expects method return types to match, found " + funcPtr.ReturnType + " on the stack", _il.Instructions(_allLocals));
                }
            }
        }

        UpdateState(OpCodes.Calli, callConventions, returnType, parameterTypes, Wrap(transitions, "CallIndirect"), arglist);

        return this;
    }

    /// <summary>
    /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
    /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
    /// </summary>
    public Emit<TDelegateType> CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
    {
        return CallIndirect(callConventions, returnType, parameterTypes, arglist: null);
    }
}
