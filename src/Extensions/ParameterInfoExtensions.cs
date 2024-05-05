using System.Diagnostics;

namespace ScrubJay.Reflection.Extensions;

public static class ParameterInfoExtensions
{
    public static RefKind RefType(this ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            if (parameter.IsIn)
                return RefKind.In;
            if (parameter.IsOut)
                return RefKind.Out;
            return RefKind.Ref;
        }
        Debug.Assert(!parameter.IsIn && !parameter.IsOut);
        return RefKind.None;
    }
    
    public static RefKind RefType(this ParameterInfo parameter, out Type parameterType)
    {
        parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            parameterType = parameterType.GetElementType()!;
            if (parameter.IsIn)
                return RefKind.In;
            if (parameter.IsOut)
                return RefKind.Out;
            return RefKind.Ref;
        }
        Debug.Assert(!parameter.IsIn && !parameter.IsOut);
        return RefKind.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParams(this ParameterInfo parameter) 
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), true);

    /// <summary>
    /// Is this <see cref="ParameterInfo"/> for an <see cref="object"/> <see cref="Array"/>?
    /// </summary>
    public static bool IsObjectArray(this ParameterInfo parameter)
    {
        return !parameter.IsIn &&
               !parameter.IsOut &&
               parameter.ParameterType == typeof(object[]);
    }
}