namespace ScrubJay.Reflection.Searching.Criteria;

public interface IMethodBaseCriterion : IGenericTypesCriterion, IMemberCriterion
{
    ICriterion<ParameterInfo[]> Parameters { get; set; }
}

public interface IMethodBaseCriterion<in TMember> : IMethodBaseCriterion,
    IMemberCriterion<TMember>
    where TMember : MethodBase
{
    
}

public record class MethodBaseCriterion<TMethod> :
    MemberCriterion<TMethod>,
    IMethodBaseCriterion<TMethod>
    where TMethod : MethodBase
{
    public ICriterion<Type[]> GenericTypes { get; set; } = Criterion.Pass<Type[]>();
    public ICriterion<ParameterInfo[]> Parameters { get; set; } = Criterion.Pass<ParameterInfo[]>();

    public MethodBaseCriterion() : base () { }
    public MethodBaseCriterion(IMemberCriterion criterion) : base(criterion) { }
    public MethodBaseCriterion(IMethodBaseCriterion criterion) : base(criterion)
    {
        this.GenericTypes = criterion.GenericTypes;
        this.Parameters = criterion.Parameters;
    }

    public override bool Matches([NotNullWhen(true)] TMethod? method)
    {
        if (!base.Matches(method))
            return false;

        if (!Parameters.Matches(method.GetParameters()))
            return false;
        
        var methodGenericTypes = method.GetGenericArguments();
        if (!GenericTypes.Matches(methodGenericTypes))
            return false;
        
        return true;
    }
}

public interface IMethodBaseCriterionBuilder<out TBuilder, TCriterion, in TMember> :
    IMemberBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMethodBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMethodBaseCriterion<TMember>
    where TMember : MethodBase
{
    TBuilder IsGeneric();
    TBuilder GenericTypes(ICriterion<Type[]> types);
    TBuilder GenericTypes(params ICriterion<Type>[] types);
    TBuilder GenericTypes(params Type[] types);

    TBuilder NoParameters();
    TBuilder Parameters(ICriterion<ParameterInfo[]> parameters);
    TBuilder Parameters(params ICriterion<ParameterInfo>[] parameters);
    TBuilder Parameters(params Type[] parameterTypes);
    TBuilder Parameters(params object?[] arguments);
    
    TBuilder Like(MethodBase method);
}

internal class MethodBaseCriterionBuilder<TBuilder, TCriterion, TMember> :
    MemberCriterionBuilder<TBuilder, TCriterion, TMember>,
    IMethodBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMethodBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMethodBaseCriterion<TMember>
    where TMember : MethodBase
{
    protected MethodBaseCriterionBuilder(TCriterion criterion) : base(criterion)
    {
    }

    public TBuilder IsGeneric()
    {
        _criterion.GenericTypes = new FuncCriterion<Type[]>(static types => types is not null && types.Length > 0);
        return _builder;
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

    public TBuilder NoParameters()
    {
        _criterion.GenericTypes = new FuncCriterion<Type[]>(static types => types is null || types.Length == 0);
        return _builder;
    }

    public TBuilder Parameters(ICriterion<ParameterInfo[]> parameters)
    {
        _criterion.Parameters = parameters;
        return _builder;
    }
    public TBuilder Parameters(params ICriterion<ParameterInfo>[] parameters)
    {
        _criterion.Parameters = Criterion.Combine(parameters);
        return _builder;
    }

    public TBuilder Parameters(params Type[] parameterTypes)
    {
        return Parameters(parameterTypes.ConvertAll<Type, ICriterion<ParameterInfo>>(static t => ParameterCriterion.Create(TypeMatchCriterion.Create(t))));
    }

    public TBuilder Parameters(params object?[] arguments)
    {
        _criterion.Parameters = Mirror.GetParameterCriteria(arguments);
        return _builder;
    }

    public TBuilder Like(MethodBase method)
    {
        base.Like(method);

        _criterion.GenericTypes = Criterion.Combine(method.GetGenericArguments()
            .ConvertAll(static type => Criterion.Match(type)));
        _criterion.Parameters = Criterion.Combine(method.GetParameters()
            .ConvertAll<ParameterInfo, ICriterion<ParameterInfo>>(static p => new ParameterCriterionBuilder().Like(p)));
        return _builder;
    }
}

public interface IMethodBaseCriterionBuilderImpl :
    IMethodBaseCriterionBuilder<IMethodBaseCriterionBuilderImpl, IMethodBaseCriterion<MethodBase>, MethodBase>;

internal class MethodBaseCriterionBuilderImpl :
    MethodBaseCriterionBuilder<IMethodBaseCriterionBuilderImpl, IMethodBaseCriterion<MethodBase>, MethodBase>,
    IMethodBaseCriterionBuilderImpl
{
    public MethodBaseCriterionBuilderImpl() : base(new MethodBaseCriterion<MethodBase>()) { }
    public MethodBaseCriterionBuilderImpl(IMemberCriterion criterion) : base(new MethodBaseCriterion<MethodBase>(criterion)) { }
    public MethodBaseCriterionBuilderImpl(IMethodBaseCriterion<MethodBase> criterion) : base(criterion) { }
}