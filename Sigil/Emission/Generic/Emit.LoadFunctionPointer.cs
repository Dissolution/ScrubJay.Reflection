using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pushes a pointer to the given function onto the stack, as a native int.
    ///
    /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
    /// </summary>
    public Emit<TDelegateType> LoadFunctionPointer(MethodInfo method)
    {
        if (method == null)
        {
            throw new ArgumentNullException("method");
        }

        var parameters = method.GetParameters();

        var paramList = (parameters).Select(p => p.ParameterType).ToArray();

        return InnerLoadFunctionPointer(method, paramList);
    }


    /// <summary>
    /// Pushes a pointer to the given function onto the stack, as a native int.
    ///
    /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
    ///
    /// This method is provided as MethodBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
    /// do not match the given method, the produced code will be invalid.
    /// </summary>
    public Emit<TDelegateType> LoadFunctionPointer(MethodBuilder method, Type[] parameterTypes)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        if(parameterTypes == null)
        {
            throw new ArgumentNullException(nameof(parameterTypes));
        }


        return InnerLoadFunctionPointer(method, parameterTypes);
    }

    Emit<TDelegateType> InnerLoadFunctionPointer(MethodInfo method, Type[] parameterTypes)
    {
        var type =
            TypeOnStack.GetKnownFunctionPointer(
                method.CallingConvention,
                HasFlag(method.CallingConvention, CallingConventions.HasThis) ? method.DeclaringType : null,
                method.ReturnType,
                parameterTypes
            );

        UpdateState(OpCodes.Ldftn, method, parameterTypes, Wrap(new[] { new StackTransition(new TypeOnStack[0], new[] { type }) }, "LoadFunctionPointer"));

        return this;
    }
}
