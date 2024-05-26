namespace ScrubJay.Reflection.Searching.Criteria;

public interface IPropertyCriterion : IMemberCriterion<PropertyInfo>
{
    ICriterion<Type> Type { get; set; }
    ICriterion<MethodInfo> Getter { get; set; }
    ICriterion<MethodInfo> Setter { get; set; }
}

public record class PropertyCriterion : MemberCriterion<PropertyInfo>, IPropertyCriterion
{
    public ICriterion<Type> Type { get; set; } = Criterion.Pass<Type>();
    public ICriterion<MethodInfo> Getter { get; set; } = Criterion.Pass<MethodInfo>();
    public ICriterion<MethodInfo> Setter { get; set; } = Criterion.Pass<MethodInfo>();

    public PropertyCriterion() : base() { }
    public PropertyCriterion(IMemberCriterion criterion) : base(criterion) { }
    public PropertyCriterion(IPropertyCriterion criterion) : base(criterion)
    {
        this.Type = criterion.Type;
        this.Getter = criterion.Getter;
        this.Setter = criterion.Setter;
    }
    
    public override bool Matches([NotNullWhen(true)] PropertyInfo? property)
    {
        if (!base.Matches(property))
            return false;

        if (!Type.Matches(property.PropertyType))
            return false;

        if (!Getter.Matches(property.GetMethod))
            return false;

        if (!Setter.Matches(property.SetMethod)) 
            return false;
        
        return true;
    }
}

public interface IPropertyCriterionBuilder<out TBuilder> : 
    IMemberBaseCriterionBuilder<TBuilder, IPropertyCriterion, PropertyInfo>
    where TBuilder : IPropertyCriterionBuilder<TBuilder>
{
    TBuilder Type(ICriterion<Type> criterion);
    TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder Type<TProperty>(TypeMatch match = TypeMatch.Exact);

    TBuilder Getter(ICriterion<MethodInfo> getter);
    TBuilder Setter(ICriterion<MethodInfo> setter);
    
    TBuilder Like(PropertyInfo property);
}

internal class PropertyCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IPropertyCriterion, PropertyInfo>,
    IPropertyCriterionBuilder<TBuilder>
    where TBuilder : IPropertyCriterionBuilder<TBuilder>
{
    protected PropertyCriterionBuilder(IPropertyCriterion criterion) : base(criterion)
    {
    }

    public TBuilder Type(ICriterion<Type> criterion)
    {
        _criterion.Type = criterion;
        return _builder;
    }
    public TBuilder Type(Type type, TypeMatch match = TypeMatch.Exact)
    {
        return Type(Criterion.Match(type, match));
    }
    public TBuilder Type<TProperty>(TypeMatch match = TypeMatch.Exact)
    {
        return Type(Criterion.Match(typeof(TProperty), match));
    }

    public TBuilder Getter(ICriterion<MethodInfo> getter)
    {
        _criterion.Getter = getter;
        return _builder;
    }
    public TBuilder Setter(ICriterion<MethodInfo> setter)
    {
        _criterion.Setter = setter;
        return _builder;
    }

    public TBuilder Like(PropertyInfo property)
    {
        base.Like(property);
        _criterion.Type = Criterion.Match(property.PropertyType);
        _criterion.Getter = property.GetMethod is null ? Criterion.IsNull<MethodInfo>() : Criterion.NotNull<MethodInfo>();
        _criterion.Setter = property.SetMethod is null ? Criterion.IsNull<MethodInfo>() : Criterion.NotNull<MethodInfo>();
        return _builder;
    }
}

public interface IPropertyCriterionBuilderImpl : 
    IPropertyCriterionBuilder<IPropertyCriterionBuilderImpl>;

internal class PropertyCriterionBuilderImpl : 
    PropertyCriterionBuilder<IPropertyCriterionBuilderImpl>,
    IPropertyCriterionBuilderImpl
{
    public PropertyCriterionBuilderImpl() : base(new PropertyCriterion()) { }
    public PropertyCriterionBuilderImpl(IMemberCriterion criterion) : base(new PropertyCriterion(criterion)) { }
    public PropertyCriterionBuilderImpl(IPropertyCriterion criterion) : base(criterion) { }
}