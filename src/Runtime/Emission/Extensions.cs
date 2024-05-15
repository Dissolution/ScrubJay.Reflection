using ScrubJay.Reflection.Info;

namespace ScrubJay.Reflection.Runtime.Emission;

public static class Extensions
{
    public static bool IsShortForm(this Label label)
    {
        var value = label.GetHashCode();
        return value <= 127 && value >= 0;
    }

    public static bool IsShortForm(this LocalVariableInfo local)
    {
        return local.LocalIndex <= byte.MaxValue;
    }

    public static OpCode GetCallOpCode(this MethodBase method)
    {
        if (method.IsConstructor)
            return OpCodes.Newobj;
        
        /* Call is for calling non-virtual, static, or superclass methods
         *   i.e. the target of the Call is not subject to overriding
         * Callvirt is for calling virtual methods
         *   i.e. the target of the Callvirt may be overridden
         *
         * Callvirt also checks for a null instance and will throw a NullReferenceException, whereas
         * Call will jump and execute.
         * We always want to use Callvirt when in doubt, it is absolutely the safest option
         */
        
        // Static?
        if (method.IsStatic) return OpCodes.Call;
        // Value-Type (all methods will automatically be sealed)
        if (method.OwnerType().IsValueType && !method.IsVirtual) return OpCodes.Call;
        
        // One would think that we could now check for sealed, but as Callvirt does a null check and instance can 
        // be null from this point on, we have to use Callvirt.
        // if (method.IsSealed()) return OpCodes.Call;
        return OpCodes.Callvirt;
    }




    public static void SetReturnSignature(this MethodBuilder methodBuilder, ReturnSignature signature)
    {
        _ = methodBuilder.DefineParameter(0, signature.GetParameterAttributes(), null);
    }

    public static void SetParameterSignature(this MethodBuilder methodBuilder, int index, ParameterSignature signature)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be zero or greater");
        
        var parameterBuilder = methodBuilder.DefineParameter(index + 1, signature.GetParameterAttributes(), signature.Name);
        if (signature.Default.IsSome(out var @default))
        {
            parameterBuilder.SetConstant(@default);
        }
    }
    
    public static void SetReturnSignature(this DynamicMethod dynamicMethod, ReturnSignature signature)
    {
        _ = dynamicMethod.DefineParameter(0, signature.GetParameterAttributes(), null);
    }

    public static void SetParameterSignature(this DynamicMethod dynamicMethod, int index, ParameterSignature signature)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be zero or greater");
        
        var parameterBuilder = dynamicMethod.DefineParameter(index + 1, signature.GetParameterAttributes(), signature.Name);
        if (signature.Default.IsSome(out var @default))
        {
            parameterBuilder?.SetConstant(@default);
        }
    }
}