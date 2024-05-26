
namespace ScrubJay.Reflection.Searching.Scratch;

public interface IMethodCriterion : IMethodBaseCriterion<MethodInfo>
{
    ICriterion<ParameterInfo>? ReturnType { get; set; }
}

public record class MethodCriterion : MethodBaseCriterion<MethodInfo>, IMethodCriterion
{
    public ICriterion<ParameterInfo>? ReturnType { get; set; }

    public MethodCriterion() : base() { }
    public MethodCriterion(IMemberCriterion criterion) : base(criterion) { }
    public MethodCriterion(IMethodBaseCriterion criterion) : base(criterion) { }
    public MethodCriterion(IMethodCriterion criterion) : base(criterion)
    {
        this.ReturnType = criterion.ReturnType;
    }
    
    public override bool Matches([NotNullWhen(true)] MethodInfo? method)
    {
        if (!base.Matches(method))
            return false;

        if (ReturnType is not null)
        {
            if (!ReturnType.Matches(method.ReturnParameter))
                return false;
        }

        return true;
    }
}


public interface IMethodCriterionBuilder<out TBuilder> : 
    IMemberCriterionBuilder<TBuilder, IMethodCriterion, MethodInfo>
    where TBuilder : IMethodCriterionBuilder<TBuilder>
{
    TBuilder ReturnType(ICriterion<ParameterInfo> criterion);
    TBuilder ReturnType(Type type, TypeMatch typeMatch = TypeMatch.Exact);
    TBuilder ReturnType<TReturn>(TypeMatch typeMatch = TypeMatch.Exact);
}

internal class MethodCriterionBuilder<TBuilder> :
    MemberCriterionBuilder<TBuilder, IMethodCriterion, MethodInfo>,
    IMethodCriterionBuilder<TBuilder>
    where TBuilder : IMethodCriterionBuilder<TBuilder>
{
    protected MethodCriterionBuilder(IMethodCriterion criterion) : base(criterion)
    {
    }

    public TBuilder ReturnType(ICriterion<ParameterInfo> criterion)
    {
        _criterion.ReturnType = criterion;
        return _builder;
    }
    public TBuilder ReturnType(Type type, TypeMatch typeMatch = TypeMatch.Exact)
        => ReturnType(new ParameterTypeMatchCriterion(type, typeMatch));
    public TBuilder ReturnType<TReturn>(TypeMatch typeMatch = TypeMatch.Exact)
        => ReturnType(new ParameterTypeMatchCriterion(typeof(TReturn), typeMatch));
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