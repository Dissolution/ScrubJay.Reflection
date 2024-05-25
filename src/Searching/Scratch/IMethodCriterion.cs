
namespace ScrubJay.Reflection.Searching.Scratch;

public interface IMethodCriterion : IMethodBaseCriterion<MethodInfo>
{
    ICriterion<ParameterInfo>? ReturnType { get; set; }
}

public record class MethodCriterion : MethodBaseCriterion<MethodInfo>, IMethodCriterion
{
    public ICriterion<ParameterInfo>? ReturnType { get; set; }

    public override bool Matches([NotNullWhen(true)] MethodInfo? method)
    {
        if (!base.Matches(method))
            return false;

        if (ReturnType is not null)
        {
            if (!ReturnType.Matches(method.ReturnParameter))
                return false;
        }

        return true;
    }
}