
namespace ScrubJay.Reflection.Searching.Criteria;

public interface IMethodCriterion : IMethodBaseCriterion<MethodInfo>
{
    ICriterion<ParameterInfo> Return { get; set; }
}

public record class MethodCriterion : MethodBaseCriterion<MethodInfo>, IMethodCriterion
{
    public ICriterion<ParameterInfo> Return { get; set; } = Criterion.Pass<ParameterInfo>();

    public MethodCriterion() : base() { }
    public MethodCriterion(IMemberCriterion criterion) : base(criterion) { }
    public MethodCriterion(IMethodBaseCriterion criterion) : base(criterion) { }
    public MethodCriterion(IMethodCriterion criterion) : base(criterion)
    {
        this.Return = criterion.Return;
    }
    
    public override bool Matches([NotNullWhen(true)] MethodInfo? method)
    {
        if (!base.Matches(method))
            return false;

        if (!Return.Matches(method.ReturnParameter))
            return false;

        return true;
    }
}


public interface IMethodCriterionBuilder<out TBuilder> : 
    IMethodBaseCriterionBuilder<TBuilder, IMethodCriterion, MethodInfo>
    where TBuilder : IMethodCriterionBuilder<TBuilder>
{
    TBuilder Returns(ICriterion<ParameterInfo> parameter);
    TBuilder Returns(Type type, TypeMatch match = TypeMatch.Exact);
    TBuilder Returns<TReturn>(TypeMatch match = TypeMatch.Exact);
    
    TBuilder Like(MethodInfo method);
}

internal class MethodCriterionBuilder<TBuilder> :
    MethodBaseCriterionBuilder<TBuilder, IMethodCriterion, MethodInfo>,
    IMethodCriterionBuilder<TBuilder>
    where TBuilder : IMethodCriterionBuilder<TBuilder>
{
    protected MethodCriterionBuilder(IMethodCriterion criterion) : base(criterion)
    {
    }

    public TBuilder Returns(ICriterion<ParameterInfo> parameter)
    {
        _criterion.Return = parameter;
        return _builder;
    }
    public TBuilder Returns(Type type, TypeMatch match = TypeMatch.Exact)
        => Returns(ParameterCriterion.Create(Criterion.Match(type, match)));
    public TBuilder Returns<TReturn>(TypeMatch match = TypeMatch.Exact)
        => Returns(ParameterCriterion.Create(Criterion.Match(typeof(TReturn), match)));

    public TBuilder Like(MethodInfo method)
    {
        base.Like(method);
        _criterion.Return = ParameterCriterion.Create(Criterion.Match(method.ReturnType));
        return _builder;
    }
}

public interface IMethodCriterionBuilderImpl : 
    IMethodCriterionBuilder<IMethodCriterionBuilderImpl>;

internal class MethodCriterionBuilderImpl : 
    MethodCriterionBuilder<IMethodCriterionBuilderImpl>,
    IMethodCriterionBuilderImpl
{
    public MethodCriterionBuilderImpl() : base(new MethodCriterion()) { }
    public MethodCriterionBuilderImpl(IMemberCriterion criterion) : base(new MethodCriterion(criterion)) { }
    public MethodCriterionBuilderImpl(IMethodCriterion criterion) : base(criterion) { }
}