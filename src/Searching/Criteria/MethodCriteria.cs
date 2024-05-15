namespace ScrubJay.Reflection.Searching.Criteria;

public record class MethodCriteria : MethodBaseCriteria, ICriteria<MethodInfo>
{
    public static new MethodCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }
    
    public override MemberTypes MemberType => MemberTypes.Method;

    public bool Matches(MethodInfo? method) => base.Matches(method);
}

public sealed class MethodCriteriaBuilder :
    MethodBaseCriteriaBuilder<MethodCriteriaBuilder, MethodCriteria>,
    ICriteria<MethodInfo>
{
    public MethodCriteriaBuilder()
    {
    }
    public MethodCriteriaBuilder(MethodCriteria criteria) : base(criteria)
    {
    }
    
    public bool Matches(MethodInfo? method) => _criteria.Matches(method);
}