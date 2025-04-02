namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class ParameterInfoExtensions
{
    /// <summary>
    /// Is this <see cref="ParameterInfo"/> declared as <c>params</c>?
    /// </summary>
    public static bool IsParams(this ParameterInfo parameter)
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), inherit: true);

    /// <summary>
    /// Deconstruct this <see cref="ParameterInfo"/> into a
    /// <see cref="ParameterReferenceKind"/> and a <see cref="Type"/>
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="paramRef"></param>
    /// <param name="paramType"></param>
    public static void Deconstruct(
        this ParameterInfo parameter,
        out ParameterReferenceKind paramRef,
        out Type paramType)
    {
        paramType = parameter.ParameterType;
        if (paramType.IsByRef)
        {
            paramRef = ParameterReferenceKind.Ref;
            paramType = paramType.GetElementType()
                .ThrowIfNull("Could not get element type of ByRef Parameter");
            
            if (parameter.IsIn)
            {
                paramRef |= ParameterReferenceKind.In;
            }
            
            if (parameter.IsOut)
            {
                paramRef |= ParameterReferenceKind.Out;
            }
        }
        else
        {
            paramRef = ParameterReferenceKind.Default;
        }
    }
   
    public static Option<object?> Default(this ParameterInfo parameter)
    {
        if (parameter.HasDefaultValue)
        {
            return Some<object?>(parameter.DefaultValue);
        }
        return None<object?>();
    }
}