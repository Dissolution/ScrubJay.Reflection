namespace ScrubJay.Reflection.Extensions;

public static class ParameterInfoExtensions
{
    public static ReferenceType RefKind(this ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            if (parameter.IsIn)
                return Reflection.ReferenceType.In;
            if (parameter.IsOut)
                return Reflection.ReferenceType.Out;
            return Reflection.ReferenceType.Ref;
        }
        Debug.Assert(!parameter.IsIn && !parameter.IsOut);
        return Reflection.ReferenceType.Default;
    }
    
    public static ReferenceType RefKind(this ParameterInfo parameter, out Type parameterType)
    {
        parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            parameterType = parameterType.GetElementType()!;
            if (parameter.IsIn)
                return Reflection.ReferenceType.In;
            if (parameter.IsOut)
                return Reflection.ReferenceType.Out;
            return Reflection.ReferenceType.Ref;
        }
        Debug.Assert(!parameter.IsIn && !parameter.IsOut);
        return Reflection.ReferenceType.Default;
    }

    /// <summary>
    /// Is this <see cref="ParameterInfo"/> declared as <c>params</c>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParams(this ParameterInfo parameter)
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), inherit: true);

    /// <summary>
    /// Is this <see cref="ParameterInfo"/>'s <see cref="ParameterInfo.ParameterType"/> <see cref="Array">object[]</see>?
    /// </summary>
    public static bool IsObjectArray(this ParameterInfo parameter)
    {
        return !parameter.IsIn &&
               !parameter.IsOut &&
               parameter.ParameterType == typeof(object[]);
    }
}