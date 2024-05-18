namespace ScrubJay.Reflection.Searching.Criteria;

public record class ParameterCriteria : Criteria, ICriteria<ParameterInfo>
{
    public static implicit operator ParameterCriteria(Type type) => Create(type);
    public static implicit operator ParameterCriteria(TypeCriteria typeCriteria) => Create(typeCriteria);

    public static ParameterCriteria Create(Type parameterType)
    {
        return new ParameterCriteria()
        {
            Type = TypeCriteria.Create(parameterType),
        };
    }
    
    public static ParameterCriteria Create(TypeCriteria parameterTypeCriteria)
    {
        return new ParameterCriteria()
        {
            Type = parameterTypeCriteria,
        };
    }
    
    public NameCriteria? Name { get; set; } = null;
    public TypeCriteria? Type { get; set; } = null;
    public RefKind RefKind { get; set; } = RefKind.Any;
    public bool? HasDefault { get; set; } = null;
    public ObjectCriteria? Default { get; set; } = null;

    public bool Matches([NotNullWhen(true)]ParameterInfo? parameter)
    {
        if (parameter is null) return false;
        if (Name is not null && !Name.Matches(parameter.Name))
            return false;
        var paramRefType = parameter.RefKind(out var parameterType);
        if (!RefKind.HasAnyFlags(paramRefType))
            return false;
        if (Type is not null && !Type.Matches(parameterType))
            return false;
        if (HasDefault.TryGetValue(out var hasDefault))
        {
            if (hasDefault != parameter.HasDefaultValue)
                return false;
            if (hasDefault && Default is not null && !Default.Matches(parameter.DefaultValue))
                return false;
        }
        return true;
    }
}

public abstract class ParameterCriteriaBuilder<TBuilder, TCriteria> : CriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : ParameterCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : ParameterCriteria, new()
{
    protected ParameterCriteriaBuilder() { }
    protected ParameterCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder Name(NameCriteria nameCriteria)
    {
        _criteria.Name = nameCriteria;
        return _builder;
    }

    public TBuilder Type(TypeCriteria typeCriteria)
    {
        _criteria.Type = typeCriteria;
        return _builder;
    }

    public TBuilder Type(RefKind refKind, TypeCriteria typeCriteria)
    {
        _criteria.RefKind = refKind;
        _criteria.Type = typeCriteria;
        return _builder;
    }

    public TBuilder Default(bool? has)
    {
        _criteria.HasDefault = has;
        return _builder;
    }

    public TBuilder Default(ObjectCriteria defaultCriteria)
    {
        _criteria.HasDefault = true;
        _criteria.Default = defaultCriteria;
        return _builder;
    }
}

public sealed class ParameterCriteriaBuilder : ParameterCriteriaBuilder<ParameterCriteriaBuilder, ParameterCriteria>, ICriteria<ParameterInfo>
{
    public bool Matches(ParameterInfo? parameter) => _criteria.Matches(parameter);
}