using RefKind = ScrubJay.Reflection.ReferenceKind;

namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class ParameterInfoExtensions
{
    /// <summary>
    /// Is this <see cref="ParameterInfo"/> declared as <c>params</c>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsParams(this ParameterInfo parameter)
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), inherit: true);

    public static RefKind ReferenceKind(this ParameterInfo parameter)
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
        return RefKind.Default;
    }

    public static RefKind ReferenceKind(this ParameterInfo parameter, out Type parameterType)
    {
        parameterType = parameter.ParameterType;
        if (parameterType.IsByRef)
        {
            parameterType = parameterType.GetElementType()!;
            Debug.Assert(parameterType is not null);
            if (parameter.IsIn)
                return RefKind.In;
            if (parameter.IsOut)
                return RefKind.Out;
            return RefKind.Ref;
        }
        Debug.Assert(!parameter.IsIn && !parameter.IsOut);
        return RefKind.Default;
    }

    public static Option<object?> DefaultOption(this ParameterInfo parameter)
    {
        if (parameter.HasDefaultValue)
        {
            return Some<object?>(parameter.DefaultValue);
        }
        return None();
    }
}