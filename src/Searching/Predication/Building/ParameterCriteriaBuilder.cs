using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public sealed class ParameterMatchCriteriaBuilder : ParameterMatchCriteriaBuilder<ParameterMatchCriteriaBuilder>
{
    public ParameterMatchCriteriaBuilder() : base()
    {
    }

    public ParameterMatchCriteriaBuilder(ParameterMatchCriteria criteria) : base(criteria)
    {
    }
}

public class ParameterMatchCriteriaBuilder<TBuilder> : MatchCriteriaBuilder<TBuilder, ParameterMatchCriteria, ParameterInfo?>
    where TBuilder : ParameterMatchCriteriaBuilder<TBuilder>
{
    public ParameterMatchCriteriaBuilder() : base(new())
    {
        
    }
    
    public ParameterMatchCriteriaBuilder(ParameterMatchCriteria criteria) : base(criteria)
    {
    }

    public TBuilder Name(ICriteria<string?> name)
    {
        _criteria.Name = name;
        return _builder;
    }
    
    public TBuilder Name(string name, StringMatchType matchType = StringMatchType.Exact, StringComparison comparison = StringComparison.Ordinal)
    {
        _criteria.Name = Criteria.Equals(name, comparison, matchType);
        return _builder;
    }
    
    public TBuilder ValueType(ICriteria<Type> type)
    {
        _criteria.ValueType = type;
        return _builder;
    }
    
    public TBuilder ValueType(Type type, TypeMatchType match = TypeMatchType.Exact)
    {
        _criteria.ValueType = Criteria.Equals(type, match);
        return _builder;
    }
    
    public TBuilder ValueType<T>(TypeMatchType match = TypeMatchType.Exact)
    {
        _criteria.ValueType = Criteria.Equals<T>(match);
        return _builder;
    }
    
    public TBuilder RefKind(RefKind kind)
    {
        _criteria.RefKind = kind;
        return _builder;
    }
    
    public TBuilder In => RefKind(Reflection.RefKind.In);
    public TBuilder Out => RefKind(Reflection.RefKind.Out);
    public TBuilder Ref => RefKind(Reflection.RefKind.Ref);
    public TBuilder NonRef => RefKind(Reflection.RefKind.None);

    // public TBuilder NoDefault
    // {
    //     get
    //     {
    //         _criteria.Default = Criterion.None<object?>();
    //         return _builder;
    //     }
    // }
    // public TBuilder HasDefault()
    // {
    //     _criteria.Default = Criterion.Some<object?>();
    //     return _builder;
    // }
    // public TBuilder HasDefault(ICriteria<object?> defaultValueCriteria)
    // {
    //     _criteria.Default = Criterion.Some<object?>(defaultValueCriteria);
    //     return _builder;
    // }
    
    public TBuilder HasAttribute(Type attributeType)
    {
        _criteria.Attributes = new FuncCriteria<Attribute[]>(attrs => attrs is not null && attrs.Any(a => a.GetType() == attributeType));
        return _builder;
    }
    
    public TBuilder HasAttribute<TAttribute>() where TAttribute : Attribute
        => HasAttribute(typeof(TAttribute));
    
    public TBuilder InheritFrom(ParameterInfo parameter)
    {
        _criteria.Name = Criteria.Equals(parameter.Name);
        _criteria.RefKind = parameter.RefKind(out var parameterType);
        _criteria.ValueType = Criteria.Equals(parameterType);
        return _builder;
    }
}