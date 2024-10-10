using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class PropertyMatchCriteriaBuilder : PropertyMatchCriteriaBuilder<PropertyMatchCriteriaBuilder>
{
    public PropertyMatchCriteriaBuilder() : base()
    {
    }

    public PropertyMatchCriteriaBuilder(PropertyMatchCriteria criteria) : base(criteria)
    {
    }

    public PropertyMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }
}

public class PropertyMatchCriteriaBuilder<TBuilder> : 
    MemberMatchCriteriaBuilder<TBuilder, PropertyMatchCriteria, PropertyInfo>
    where TBuilder : PropertyMatchCriteriaBuilder<TBuilder>
{
    public PropertyMatchCriteriaBuilder() : base()
    {
    }

    public PropertyMatchCriteriaBuilder(PropertyMatchCriteria criteria) : base(criteria)
    {
    }

    public PropertyMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }


    public TBuilder ValueType(ICriteria<Type> criteria)
    {
        _criteria.ValueType = criteria;
        return _builder;
    }
    public TBuilder ValueType(Type type, TypeMatchType match = TypeMatchType.Exact)
    {
        return ValueType(new TypeEqualityCriteria(type, match));
    }
    public TBuilder ValueType<TProperty>(TypeMatchType match = TypeMatchType.Exact)
    {
        return ValueType(new TypeEqualityCriteria(typeof(TProperty), match));
    }

    public TBuilder Getter(ICriteria<MethodInfo?> getter)
    {
        _criteria.Getter = getter;
        return _builder;
    }
    public TBuilder Setter(ICriteria<MethodInfo?> setter)
    {
        _criteria.Setter = setter;
        return _builder;
    }

    public TBuilder InheritFrom(PropertyInfo property)
    {
        base.InheritFrom(property);
        _criteria.ValueType = new TypeEqualityCriteria(property.PropertyType);
        _criteria.Getter = property.GetMethod is null ? Criteria<MethodInfo?>.IsNull : Criteria<MethodInfo?>.IsNotNull;
        _criteria.Setter = property.SetMethod is null  ? Criteria<MethodInfo?>.IsNull : Criteria<MethodInfo?>.IsNotNull;
        return _builder;
    }
}