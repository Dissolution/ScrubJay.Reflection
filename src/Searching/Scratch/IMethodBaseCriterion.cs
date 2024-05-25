namespace ScrubJay.Reflection.Searching.Scratch;

public interface IMethodBaseCriterion<in TMember> : 
    IMemberCriterion<TMember>,
    IGenericTypesCriterion
    where TMember : MethodBase
{
    ICriterion<ParameterInfo>[]? Parameters { get; set; }
}

public record class MethodBaseCriterion<TMethod> :
    MemberCriterion<TMethod>,
    IMethodBaseCriterion<TMethod>
    where TMethod : MethodBase
{
    public ICriterion<Type>[]? GenericTypes { get; set; }
    public ICriterion<ParameterInfo>[]? Parameters { get; set; }

    public override bool Matches([NotNullWhen(true)] TMethod? method)
    {
        if (!base.Matches(method))
            return false;
        
        if (Parameters is not null)
        {
            var paramz = method.GetParameters();
            if (paramz.Length != Parameters.Length)
                return false;
            for (var i = 0; i < paramz.Length; i++)
            {
                if (!Parameters[i].Matches(paramz[i]))
                    return false;
            }
        }
        if (GenericTypes is not null)
        {
            var methodGenericTypes = method.GetGenericArguments();
            if (methodGenericTypes.Length != GenericTypes.Length)
                return false;
            for (var i = 0; i < methodGenericTypes.Length; i++)
            {
                if (!GenericTypes[i].Matches(methodGenericTypes[i]))
                    return false;
            }
        }
        return true;
    }
}