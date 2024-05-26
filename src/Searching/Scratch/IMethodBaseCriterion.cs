namespace ScrubJay.Reflection.Searching.Scratch;

public interface IMethodBaseCriterion : IGenericTypesCriterion, IMemberCriterion
{
    ICriterion<ParameterInfo>[]? Parameters { get; set; }
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
    public ICriterion<Type>[]? GenericTypes { get; set; }
    public ICriterion<ParameterInfo>[]? Parameters { get; set; }

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

        if (Parameters is not null)
        {
            var paramz = method.GetParameters();
            if (paramz.Length != Parameters.Length)
                return false;
            for (var i = 0; i < paramz.Length; i++)
            {
                if (!Parameters[i].Matches(paramz[i]))
                    return false;
            }
        }
        if (GenericTypes is not null)
        {
            var methodGenericTypes = method.GetGenericArguments();
            if (methodGenericTypes.Length != GenericTypes.Length)
                return false;
            for (var i = 0; i < methodGenericTypes.Length; i++)
            {
                if (!GenericTypes[i].Matches(methodGenericTypes[i]))
                    return false;
            }
        }
        return true;
    }
}

public interface IMethodBaseCriterionBuilder<out TBuilder, TCriterion, in TMember> :
    IMemberCriterionBuilder<TBuilder, TCriterion, TMember>
    where TBuilder : IMethodBaseCriterionBuilder<TBuilder, TCriterion, TMember>
    where TCriterion : IMethodBaseCriterion<TMember>
    where TMember : MethodBase
{
    TBuilder GenericTypes(params ICriterion<Type>[] types);
    TBuilder GenericTypes(params Type[] genericTypes);
    TBuilder Parameters(params ICriterion<ParameterInfo>[] parameters);
    TBuilder Parameters(params Type[] parameterTypes);
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

    public TBuilder GenericTypes(params ICriterion<Type>[] types)
    {
        _criterion.GenericTypes = types;
        return _builder;
    }

    public TBuilder GenericTypes(params Type[] genericTypes)
    {
        _criterion.GenericTypes = genericTypes.ConvertAll<Type, ICriterion<Type>>(static t => new TypeMatchCriterion(t));
        return _builder;
    }

    public TBuilder Parameters(params ICriterion<ParameterInfo>[] parameters)
    {
        _criterion.Parameters = parameters;
        return _builder;
    }

    public TBuilder Parameters(params Type[] parameterTypes)
    {
        _criterion.Parameters = parameterTypes.ConvertAll<Type, ICriterion<ParameterInfo>>(static t => new ParameterTypeMatchCriterion(t));
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