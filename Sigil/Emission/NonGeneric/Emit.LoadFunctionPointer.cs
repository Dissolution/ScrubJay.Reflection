namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pushes a pointer to the given function onto the stack, as a native int.
    /// 
    /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
    /// </summary>
    public Emit LoadFunctionPointer(MethodInfo method)
    {
        _innerEmit.LoadFunctionPointer(method);
        return this;
    }

    /// <summary>
    /// Pushes a pointer to the given function onto the stack, as a native int.
    /// 
    /// To resolve a method at runtime using an object, use LoadVirtualFunctionPointer instead.
    /// 
    /// This method is provided as MethodBuilder cannot be inspected for parameter information at runtime.  If the passed parameterTypes
    /// do not match the given method, the produced code will be invalid.
    /// </summary>
    public Emit LoadFunctionPointer(MethodBuilder method, Type[] parameterTypes)
    {
        _innerEmit.LoadFunctionPointer(method, parameterTypes);
        return this;
    }
}