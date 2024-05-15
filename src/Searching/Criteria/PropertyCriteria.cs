namespace ScrubJay.Reflection.Searching.Criteria;

public record class PropertyCriteria : MemberCriteria, ICriteria<PropertyInfo>
{
    public static PropertyCriteria Create(TypeCriteria propertyType)
    {
        return new()
        {
            Type = propertyType,
        };
    }
    public static new PropertyCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }
    
    
    public TypeCriteria? Type { get; set; } = null;
    public bool? HasGetter { get; set; } = null;
    public MethodBaseCriteria? Getter { get; set; } = null;
    public bool? HasSetter { get; set; } = null;
    public MethodBaseCriteria? Setter { get; set; } = null;

    public override MemberTypes MemberType => MemberTypes.Property;

    public bool Matches(PropertyInfo? property)
    {
        if (!base.Matches(property))
            return false;
        if (Type is not null && !Type.Matches(property.PropertyType))
            return false;

        var getMethod = property.GetMethod;
        if (HasGetter.TryGetValue(out var hasGetter) &&
            hasGetter != (getMethod is not null))
            return false;
        if (Getter is not null)
        {
            if (getMethod is null) return false;
            if (!Getter.Matches(getMethod)) return false;
        }
        
        var setMethod = property.SetMethod;
        if (HasSetter.TryGetValue(out var hasSetter) &&
            hasSetter != (setMethod is not null))
            return false;
        if (Setter is not null)
        {
            if (setMethod is null) return false;
            if (!Setter.Matches(setMethod)) return false;
        }
        
        return true;
    }
}

public abstract class PropertyCriteriaBuilder<TBuilder, TCriteria> : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : PropertyCriteria, new()
{
    protected PropertyCriteriaBuilder() { }
    protected PropertyCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder Type(TypeCriteria typeCriteria)
    {
        _criteria.Type = typeCriteria;
        return _builder;
    }

    public TBuilder Getter(bool? has)
    {
        _criteria.HasGetter = has;
        return _builder;
    }

    public TBuilder Getter(MethodBaseCriteria methodCriteria)
    {
        _criteria.HasGetter = true;
        _criteria.Getter = methodCriteria;
        return _builder;
    }
    
    public TBuilder Setter(bool? has)
    {
        _criteria.HasSetter = has;
        return _builder;
    }

    public TBuilder Setter(MethodBaseCriteria methodCriteria)
    {
        _criteria.HasSetter = true;
        _criteria.Setter = methodCriteria;
        return _builder;
    }
}

public sealed class PropertyCriteriaBuilder : PropertyCriteriaBuilder<PropertyCriteriaBuilder, PropertyCriteria>, ICriteria<PropertyInfo>
{
    internal PropertyCriteriaBuilder()
    {
    }
    internal PropertyCriteriaBuilder(PropertyCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(PropertyInfo? property) => _criteria.Matches(property);
}