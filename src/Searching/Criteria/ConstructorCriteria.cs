namespace ScrubJay.Reflection.Searching.Criteria;

public record class ConstructorCriteria : MethodBaseCriteria, ICriteria<ConstructorInfo>
{
    public static new ConstructorCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }
    
    public override MemberTypes MemberType => MemberTypes.Constructor;

    public bool Matches(ConstructorInfo? ctor) => base.Matches(ctor);
}

public sealed class ConstructorCriteriaBuilder :
    MethodBaseCriteriaBuilder<ConstructorCriteriaBuilder, ConstructorCriteria>,
    ICriteria<ConstructorInfo>
{
    public ConstructorCriteriaBuilder()
    {
    }
    public ConstructorCriteriaBuilder(ConstructorCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(ConstructorInfo? ctor) => _criteria.Matches(ctor);
}