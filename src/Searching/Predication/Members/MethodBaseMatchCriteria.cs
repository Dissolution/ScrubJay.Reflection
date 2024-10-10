namespace ScrubJay.Reflection.Searching.Predication.Members;

public record class MethodBaseMatchCriteria<TMethod> : MemberMatchCriteria<TMethod>
    where TMethod : MethodBase
{
    public ICriteria<Type[]>? GenericTypes { get; set; }
    public ICriteria<ParameterInfo[]>? Parameters { get; set; }

    public MethodBaseMatchCriteria()
    {
    }

    public override bool Matches([NotNullWhen(true)] TMethod? method)
    {
        if (!base.Matches(method))
            return false;

        if (!Matches(Parameters, method.GetParameters))
            return false;
        
        if (!Matches(GenericTypes, method.GetGenericArguments))
            return false;
        
        return true;
    }
}