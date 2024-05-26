namespace ScrubJay.Reflection.Searching.Criteria;

public interface ITypeCriterion : IMemberCriterion<Type>, 
    IGenericTypesCriterion
{

}

public record class TypeCriterion : MemberCriterion<Type>, ITypeCriterion
{
    public ICriterion<Type[]> GenericTypes { get; set; } = Criterion.Pass<Type[]>();

    public TypeCriterion() : base() { }
    public TypeCriterion(IMemberCriterion criterion) : base(criterion) { }
    public TypeCriterion(ITypeCriterion criterion) : base(criterion)
    {
        this.GenericTypes = criterion.GenericTypes;
    }
    
    public override bool Matches([NotNullWhen(true)] Type? type)
    {
        if (!base.Matches(type))
            return false;

        if (!GenericTypes.Matches(type.GenericTypeArguments))
            return false;

        return true;
    }
}

public interface ITypeCriterionBuilder<out TBuilder> : 
    IMemberBaseCriterionBuilder<TBuilder, ITypeCriterion, Type>
    where TBuilder : ITypeCriterionBuilder<TBuilder>
{
    TBuilder GenericTypes(ICriterion<Type[]> types);
    TBuilder GenericTypes(params ICriterion<Type>[] types);
    TBuilder GenericTypes(params Type[] types);
    
    TBuilder Like(Type type);
}

internal class TypeCriterionBuilder<TBuilder> : MemberCriterionBuilder<TBuilder, ITypeCriterion, Type>, 
    ITypeCriterionBuilder<TBuilder>
    where TBuilder : ITypeCriterionBuilder<TBuilder>
{
    protected TypeCriterionBuilder(ITypeCriterion criterion) : base(criterion)
    {
    }

    public TBuilder GenericTypes(ICriterion<Type[]> types)
    {
        _criterion.GenericTypes = types;
        return _builder;
    }
    
    public TBuilder GenericTypes(params ICriterion<Type>[] types)
    {
        _criterion.GenericTypes = Criterion.Combine(types);
        return _builder;
    }

    public TBuilder GenericTypes(params Type[] genericTypes)
    {
        return GenericTypes(genericTypes.ConvertAll<Type, ICriterion<Type>>(static t => Criterion.Match(t)));
    }

    public TBuilder Like(Type type)
    {
        base.Like(type);
        _criterion.GenericTypes = Criterion.Combine(type.GetGenericArguments()
            .ConvertAll(static type => Criterion.Match(type)));
        return _builder;
    }
}

public interface ITypeCriterionBuilderImpl : ITypeCriterionBuilder<ITypeCriterionBuilderImpl>;

internal class TypeCriterionBuilderImpl : TypeCriterionBuilder<ITypeCriterionBuilderImpl>,
    ITypeCriterionBuilderImpl
{
    public TypeCriterionBuilderImpl() : base(new TypeCriterion()) { }
    public TypeCriterionBuilderImpl(IMemberCriterion criterion) : base(new TypeCriterion(criterion)) { }
    public TypeCriterionBuilderImpl(ITypeCriterion criterion) : base(criterion) { }
}