namespace ScrubJay.Reflection.Searching.Scratch;

public interface IConstructorCriterion : IMethodBaseCriterion<ConstructorInfo>
{
    ICriterion<Type>? Type { get; set; }
}

public record class ConstructorCriterion : MethodBaseCriterion<ConstructorInfo>, IConstructorCriterion
{
    public ICriterion<Type>? Type { get; set; }

    public ConstructorCriterion() : base() { }
    public ConstructorCriterion(IMemberCriterion criterion) : base(criterion) { }
    public ConstructorCriterion(IMethodBaseCriterion criterion) : base(criterion) { }
    public ConstructorCriterion(IConstructorCriterion criterion) : base(criterion)
    {
        this.Type = criterion.Type;
    }
    
    public override bool Matches(ConstructorInfo? ctor)
    {
        if (!base.Matches(ctor))
            return false;

        if (Type is not null && !Type.Matches(ctor.DeclaringType))
            return false;

        return true;
    }
}


public interface IConstructorCriterionBuilder<out TBuilder> : 
    IMethodBaseCriterionBuilder<TBuilder, IConstructorCriterion, ConstructorInfo>
    where TBuilder : IConstructorCriterionBuilder<TBuilder>
{
    TBuilder Type(ICriterion<Type> criterion);
    TBuilder Type(Type instanceType, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder Type<TInstance>(TypeMatch typeMatch = TypeMatch.Exact);
}

internal class ConstructorCriterionBuilder<TBuilder> :
    MethodBaseCriterionBuilder<TBuilder, IConstructorCriterion, ConstructorInfo>,
    IConstructorCriterionBuilder<TBuilder>
    where TBuilder : IConstructorCriterionBuilder<TBuilder>
{
    protected ConstructorCriterionBuilder(IConstructorCriterion criterion) : base(criterion)
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
    public TBuilder Type<TInstance>(TypeMatch typeMatch = TypeMatch.Exact)
    {
        return Type(new TypeMatchCriterion(typeof(TInstance), typeMatch));
    }
}

public interface IConstructorCriterionBuilderImpl : 
    IConstructorCriterionBuilder<IConstructorCriterionBuilderImpl>;

internal class ConstructorCriterionBuilderImpl : 
    ConstructorCriterionBuilder<IConstructorCriterionBuilderImpl>,
    IConstructorCriterionBuilderImpl
{
    public ConstructorCriterionBuilderImpl() : base(new ConstructorCriterion()) { }
    public ConstructorCriterionBuilderImpl(IMemberCriterion criterion) : base(new ConstructorCriterion(criterion)) { }
    public ConstructorCriterionBuilderImpl(IConstructorCriterion criterion) : base(criterion) { }
}