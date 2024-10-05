namespace ScrubJay.Reflection.Searching.Criteria;

public interface IParameterCriterion : ICriterion<ParameterInfo>,
    IHasAttributesCriterion
{
    ICriterion<string> Name { get; set; }
    ICriterion<Type> Type { get; set; }
    RefKind RefKind { get; set; }
    ICriterion<Option<object?>> Default { get; set; }
}

public record class ParameterCriterion : IParameterCriterion
{
    public static ICriterion<ParameterInfo> Create(ICriterion<Type> type)
    {
        return new SubTypeMatchCriterion<ParameterInfo>(type, static p => p?.ParameterType);
    }
    
    public ICriterion<string> Name { get; set; } = Criterion.Pass<string>();
    public ICriterion<Type> Type { get; set; } = Criterion.Pass<Type>();
    public RefKind RefKind { get; set; } = RefKind.Any;
    public ICriterion<Option<object?>> Default { get; set; } = Criterion.Pass<Option<object?>>();

    public ICriterion<Attribute[]> Attributes { get; set; } = Criterion.Pass<Attribute[]>();

    public bool Matches(ParameterInfo? parameter)
    {
        if (parameter is null) 
            return false;
        
        if (!Name.Matches(parameter.Name))
            return false;
        
        var paramRefType = parameter.RefKind(out var parameterType);
        if (!RefKind.HasAnyFlags(paramRefType))
            return false;
        
        if (!Type.Matches(parameterType))
            return false;

        Option<object?> defaultValue = parameter.HasDefaultValue ? Option<object?>.Some(parameter.DefaultValue) : Option<object?>.None();
        if (!Default.Matches(defaultValue))
            return false;
        
        if (!Attributes.Matches(Attribute.GetCustomAttributes(parameter)))
            return false;
        
        return true;
    }
}

public interface IParameterCriterionBuilder<out TBuilder> : 
    IHasAttributesCriterionBuilder<TBuilder>,
    ICriterion<ParameterInfo>
    where TBuilder : IParameterCriterionBuilder<TBuilder>
{
    TBuilder Name(ICriterion<string> name);
    TBuilder Name(string name, StringMatch match = StringMatch.Exact, StringComparison comparison = StringComparison.Ordinal);

    TBuilder Type(ICriterion<Type> type);
    TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder Type<T>(TypeMatch match = TypeMatch.Exact);

    TBuilder RefKind(RefKind kind);
    TBuilder In { get; }
    TBuilder Out { get; }
    TBuilder Ref { get; }
    TBuilder NonRef { get; }

    TBuilder NoDefault { get; }
    TBuilder HasDefault();
    TBuilder HasDefault(ICriterion<object?> defaultValueCriterion);
    
    TBuilder Like(ParameterInfo parameter);
}

internal class ParameterCriterionBuilder<TBuilder> : IParameterCriterionBuilder<TBuilder>
    where TBuilder : IParameterCriterionBuilder<TBuilder>
{
    protected readonly TBuilder _builder;
    protected readonly IParameterCriterion _criterion;
    
    protected ParameterCriterionBuilder(IParameterCriterion criterion)
    {
        _builder = (TBuilder)(IParameterCriterionBuilder<TBuilder>)this;
        _criterion = criterion;
    }

    public bool Matches(ParameterInfo? parameter)
    {
        return _criterion.Matches(parameter);
    }

    public TBuilder Name(ICriterion<string> name)
    {
        _criterion.Name = name;
        return _builder;
    }
    
    public TBuilder Name(string name, StringMatch match = StringMatch.Exact, StringComparison comparison = StringComparison.Ordinal)
    {
        _criterion.Name = Criterion.Match(name, match, comparison);
        return _builder;
    }
    
    public TBuilder Type(ICriterion<Type> type)
    {
        _criterion.Type = type;
        return _builder;
    }
    
    public TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact)
    {
        _criterion.Type = Criterion.Match(type, match);
        return _builder;
    }
    
    public TBuilder Type<T>(TypeMatch match = TypeMatch.Exact)
    {
        _criterion.Type = Criterion.Match(typeof(T), match);
        return _builder;
    }
    
    public TBuilder RefKind(RefKind kind)
    {
        _criterion.RefKind = kind;
        return _builder;
    }
    public TBuilder In => RefKind(Reflection.RefKind.In);
    public TBuilder Out => RefKind(Reflection.RefKind.Out);
    public TBuilder Ref => RefKind(Reflection.RefKind.Ref);
    public TBuilder NonRef => RefKind(Reflection.RefKind.None);

    public TBuilder NoDefault
    {
        get
        {
            _criterion.Default = Criterion.None<object?>();
            return _builder;
        }
    }
    public TBuilder HasDefault()
    {
        _criterion.Default = Criterion.Some<object?>();
        return _builder;
    }
    public TBuilder HasDefault(ICriterion<object?> defaultValueCriterion)
    {
        _criterion.Default = Criterion.Some<object?>(defaultValueCriterion);
        return _builder;
    }
    public TBuilder HasAttribute(Type attributeType)
    {
        _criterion.Attributes = new FuncCriterion<Attribute[]>(attrs => attrs is not null && attrs.Any(a => a.GetType() == attributeType));
        return _builder;
    }
    public TBuilder HasAttribute<TAttribute>() where TAttribute : Attribute
        => HasAttribute(typeof(TAttribute));
    
    public TBuilder Like(ParameterInfo parameter)
    {
        _criterion.Name = Criterion.Match(parameter.Name);
        _criterion.RefKind = parameter.RefKind(out var parameterType);
        _criterion.Type = Criterion.Match(parameterType);
        return _builder;
    }
}

public interface IParameterCriterionBuilder : IParameterCriterionBuilder<IParameterCriterionBuilder>;

internal class ParameterCriterionBuilder : ParameterCriterionBuilder<IParameterCriterionBuilder>, IParameterCriterionBuilder
{
    public ParameterCriterionBuilder() : base(new ParameterCriterion()) { }
    public ParameterCriterionBuilder(IParameterCriterion criterion) :  base(criterion) { }
}