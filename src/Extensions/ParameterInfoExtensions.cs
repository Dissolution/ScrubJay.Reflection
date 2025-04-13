namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class ParameterInfoExtensions
{
    /// <summary>
    /// Is this <see cref="ParameterInfo"/> declared as <c>params</c>?
    /// </summary>
    public static bool IsParams(this ParameterInfo parameter)
        => Attribute.IsDefined(parameter, typeof(ParamArrayAttribute), inherit: true);

    public static TRK TypeRefKind(this ParameterInfo? parameter)
    {
        TRK kind = TRK.Default;
        if (parameter is null)
            return kind;
        
        var paramType = parameter.ParameterType;
        if (paramType.IsByRef)
        {
            kind = TRK.Ref;

            if (parameter.IsIn)
            {
                kind |= TRK.In;
            }
            
            if (parameter.IsOut)
            {
                kind |= TRK.Out;
            }
        }

        return kind;
    }
    
    /// <summary>
    /// Deconstruct this <see cref="ParameterInfo"/> into a
    /// <see cref="TypeRefKind"/> and a <see cref="Type"/>
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="paramRef"></param>
    /// <param name="paramType"></param>
    public static void Deconstruct(
        this ParameterInfo parameter,
        out TRK paramRef,
        out Type paramType)
    {
        paramType = parameter.ParameterType;
        if (paramType.IsByRef)
        {
            paramRef = TRK.Ref;
            paramType = paramType.GetElementType()
                .ThrowIfNull("Could not get element type of ByRef Parameter");
            
            if (parameter.IsIn)
            {
                paramRef |= TRK.In;
            }
            
            if (parameter.IsOut)
            {
                paramRef |= TRK.Out;
            }
        }
        else
        {
            paramRef = TRK.Default;
        }
    }
   
    public static Option<object?> Default(this ParameterInfo parameter)
    {
        try
        {
            if (parameter.HasDefaultValue)
            {
                return Some<object?>(parameter.DefaultValue);
            }
            return None<object?>();
        }
        catch (Exception ex)
        {
            return None();
        }
    }

    public static bool CanAccept(this ParameterInfo parameter, object? arg)
    {
        var paramType = parameter.ParameterType;
        
        if (arg is null)
        {
            if (paramType.CanContainNull() && 
                parameter.NullabilityInfo()?.WriteState != NullabilityState.NotNull)
            {
                return true;
            }
            return false;
        }
        
        return arg.GetType().Implements(paramType);
    }
}