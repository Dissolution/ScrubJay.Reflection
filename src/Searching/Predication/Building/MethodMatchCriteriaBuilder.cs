using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class MethodMatchCriteriaBuilder : MethodMatchCriteriaBuilder<MethodMatchCriteriaBuilder>
{
    public MethodMatchCriteriaBuilder() : base()
    {
    }

    public MethodMatchCriteriaBuilder(MethodMatchCriteria criteria) : base(criteria)
    {
    }

    public MethodMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class MethodMatchCriteriaBuilder<TBuilder> : MethodBaseMatchCriteriaBuilder<TBuilder, MethodMatchCriteria, MethodInfo>
    where TBuilder : MethodMatchCriteriaBuilder<TBuilder>
{
    public MethodMatchCriteriaBuilder() : base()
    {
    }

    public MethodMatchCriteriaBuilder(MethodMatchCriteria criteria) : base(criteria)
    {
    }

    public MethodMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder Returns(ICriteria<ParameterInfo> parameter)
    {
        _criteria.Return = parameter;
        return _builder;
    }

    public TBuilder Returns(Type type, TypeMatchType match = TypeMatchType.Exact)
        => Returns(
            new ParameterMatchCriteria()
            {
                ValueType = Criteria.Equals(type, match),
            });
    
    public TBuilder Returns<TReturn>(TypeMatchType match = TypeMatchType.Exact)
        => Returns(
            new ParameterMatchCriteria()
            {
                ValueType = Criteria.Equals<TReturn>(match),
            });

    public TBuilder InheritFrom(MethodInfo method)
    {
        base.InheritFrom(method);
        _criteria.Return = new ParameterMatchCriteria(method.ReturnParameter);
        return _builder;
    }
}