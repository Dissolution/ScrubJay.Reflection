using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class ConstructorMatchCriteriaBuilder : ConstructorMatchCriteriaBuilder<ConstructorMatchCriteriaBuilder>
{
    public ConstructorMatchCriteriaBuilder() : base()
    {
    }

    public ConstructorMatchCriteriaBuilder(ConstructorMatchCriteria criteria) : base(criteria)
    {
    }

    public ConstructorMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class ConstructorMatchCriteriaBuilder<TBuilder> : MethodBaseMatchCriteriaBuilder<TBuilder, ConstructorMatchCriteria, ConstructorInfo>
    where TBuilder : ConstructorMatchCriteriaBuilder<TBuilder>
{
    public ConstructorMatchCriteriaBuilder() : base()
    {
    }

    public ConstructorMatchCriteriaBuilder(ConstructorMatchCriteria criteria) : base(criteria)
    {
    }

    public ConstructorMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder ConstructedType(ICriteria<Type> criteria)
    {
        _criteria.ConstructedType = criteria;
        return _builder;
    }
    public TBuilder ConstructedType(Type type, TypeMatchType typeMatch = TypeMatchType.Exact)
    {
        return ConstructedType(Criteria.Equals(type, typeMatch));
    }
    public TBuilder ConstructedType<TInstance>(TypeMatchType typeMatch = TypeMatchType.Exact)
    {
        return ConstructedType(Criteria.Equals<TInstance>(typeMatch));
    }

    public TBuilder InheritFrom(ConstructorInfo ctor)
    {
        base.InheritFrom(ctor);
        _criteria.ConstructedType = Criteria.Equals(ctor.DeclaringType!);
        return _builder;
    }
}