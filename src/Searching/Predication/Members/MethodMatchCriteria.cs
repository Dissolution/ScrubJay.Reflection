
namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class MethodMatchCriteria : MethodBaseMatchCriteria<MethodInfo>
{
    public ICriteria<ParameterInfo>? Return { get; set; }
    
    public override bool Matches([NotNullWhen(true)] MethodInfo? method)
    {
        if (!base.Matches(method))
            return false;

        if (!Matches(Return, method.ReturnParameter))
            return false;

        return true;
    }
}