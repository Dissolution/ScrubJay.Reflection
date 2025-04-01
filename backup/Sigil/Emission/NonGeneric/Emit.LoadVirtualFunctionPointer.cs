namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
    /// 
    /// For static or non-virtual functions, use LoadFunctionPointer
    /// </summary>
    public Emit LoadVirtualFunctionPointer(MethodInfo method)
    {
        _innerEmit.LoadVirtualFunctionPointer(method);
        return this;
    }

    /// <summary>
    /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
    /// 
    /// For static or non-virtual functions, use LoadFunctionPointer.
    /// 
    /// This method is provided as MethodBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
    /// do not match the given method, the produced code will be invalid.
    /// </summary>
    public Emit LoadVirtualFunctionPointer(MethodBuilder method, Type[] parameterTypes)
    {
        _innerEmit.LoadVirtualFunctionPointer(method, parameterTypes);
        return this;
    }
}