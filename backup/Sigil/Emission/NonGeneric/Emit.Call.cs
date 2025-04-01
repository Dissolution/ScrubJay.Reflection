namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Calls the given method.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
    /// 
    /// If the given method is an instance method, the `this` reference should appear before any parameters.
    /// 
    /// Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.
    /// 
    /// To call overrides of instance methods, use CallVirtual.
    /// 
    /// When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.
    /// </summary>
    public Emit Call(MethodInfo method, Type[] arglist = null)
    {
        _innerEmit.Call(method, arglist);
        return this;
    }


    /// <summary>
    /// Calls the given constructor.  Pops its arguments in reverse order (left-most deepest in the stack).
    /// 
    /// The `this` reference should appear before any parameters.
    /// </summary>
    public Emit Call(ConstructorInfo constructor)
    {
        _innerEmit.Call(constructor);
        return this;
    }

    /// <summary>
    /// Calls the method being constructed by the given emit.  Emits so used must have been constructed with BuildMethod or related methods.
    /// 
    /// Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
    /// 
    /// If the given method is an instance method, the `this` reference should appear before any parameters.
    /// 
    /// Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.
    /// 
    /// To call overrides of instance methods, use CallVirtual.
    /// Recursive calls can only be performed with DynamicMethods, other passed in Emits must already have their methods created.
    /// When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.
    /// </summary>
    public Emit Call(Emit emit, Type[] arglist = null)
    {
        if (emit == null)
        {
            throw new ArgumentNullException("emit");
        }
            
        MethodInfo methodInfo = emit._innerEmit.MtdBuilder ?? (MethodInfo)emit._innerEmit.DynMethod;
        if (methodInfo == null)
        {
            var dynMethod = new System.Reflection.Emit.DynamicMethod(emit._name, emit._returnType, emit._parameterTypes, emit._module, skipVisibility: true);

            emit._innerEmit.DynMethod = dynMethod;
            methodInfo = dynMethod;
        }

        _innerEmit.Call(emit._innerEmit, arglist);
        return this;
    }
}