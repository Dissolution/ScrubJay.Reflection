namespace ScrubJay.Reflection.Searching.Scratch;

public interface IPropertyCriterion : IMemberCriterion<PropertyInfo>
{
    ICriterion<Type>? Type { get; set; }
    ICriterion<MethodInfo>? Getter { get; set; }
    ICriterion<MethodInfo>? Setter { get; set; }
}

public record class PropertyCriterion : MemberCriterion<PropertyInfo>, IPropertyCriterion
{
    public ICriterion<Type>? Type { get; set; }
    public ICriterion<MethodInfo>? Getter { get; set; }
    public ICriterion<MethodInfo>? Setter { get; set; }

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

        if (Type is not null && !Type.Matches(property.PropertyType))
            return false;

        if (Getter is not null)
        {
            var getMethod = property.GetMethod;
            if (getMethod is null)
                return false;
            if (!Getter.Matches(getMethod))
                return false;
        }

        if (Setter is not null)
        {
            var setMethod = property.SetMethod;
            if (setMethod is null) 
                return false;
            if (!Setter.Matches(setMethod)) 
                return false;
        }

        return true;
    }
}

public interface IPropertyCriterionBuilder<out TBuilder> : 
    IMemberCriterionBuilder<TBuilder, IPropertyCriterion, PropertyInfo>
    where TBuilder : IPropertyCriterionBuilder<TBuilder>
{
    TBuilder Type(ICriterion<Type> criterion);
    TBuilder Type(Type type, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder Type<TProperty>(TypeMatch typeMatch = TypeMatch.Exact);

    TBuilder Getter(ICriterion<MethodInfo> getter);
    TBuilder Setter(ICriterion<MethodInfo> setter);
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
    public TBuilder Type(Type type, TypeMatch typeMatch = TypeMatch.Exact)
    {
        return Type(new TypeMatchCriterion(type, typeMatch));
    }
    public TBuilder Type<TField>(TypeMatch typeMatch = TypeMatch.Exact)
    {
        return Type(new TypeMatchCriterion(typeof(TField), typeMatch));
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