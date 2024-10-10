using ScrubJay.Reflection.Searching.Predication.Members;

namespace ScrubJay.Reflection.Searching.Predication.Building;

public class MethodBaseMatchCriteriaBuilder<TBuilder, TCriteria, TMethod> :
    MemberMatchCriteriaBuilder<TBuilder, TCriteria, TMethod>
    where TBuilder : MethodBaseMatchCriteriaBuilder<TBuilder, TCriteria, TMethod>
    where TCriteria : MethodBaseMatchCriteria<TMethod>, new()
    where TMethod : MethodBase
{
    public MethodBaseMatchCriteriaBuilder() : base()
    {
    }

    public MethodBaseMatchCriteriaBuilder(TCriteria criteria) : base(criteria)
    {
    }

    public MethodBaseMatchCriteriaBuilder(MemberMatchCriteria<MemberInfo> criteria) : base(criteria)
    {
    }

    public TBuilder IsGeneric()
    {
        _criteria.GenericTypes = Criteria.IsNotNullOrEmpty<Type[]>();
        return _builder;
    }

    public TBuilder GenericTypes(ICriteria<Type[]> types)
    {
        _criteria.GenericTypes = types;
        return _builder;
    }
    
    public TBuilder GenericTypes(params ICriteria<Type>[] types)
    {
        _criteria.GenericTypes = Criteria<Type>.ArrayOfCriteriaToCriteriaOfArray(types);
        return _builder;
    }

    public TBuilder GenericTypes(params Type[] genericTypes)
    {
        return GenericTypes(genericTypes.ConvertAll<Type, ICriteria<Type>>(static t => Criteria.Equals(t)));
    }

    public TBuilder NoParameters()
    {
        _criteria.GenericTypes = new FuncCriteria<Type[]>(static types => types is null || types.Length == 0);
        return _builder;
    }

    public TBuilder Parameters(ICriteria<ParameterInfo[]> parameters)
    {
        _criteria.Parameters = parameters;
        return _builder;
    }
    public TBuilder Parameters(params ICriteria<ParameterInfo>[] parameters)
    {
        _criteria.Parameters = Criteria<ParameterInfo>.ArrayOfCriteriaToCriteriaOfArray(parameters);
        return _builder;
    }

    public TBuilder Parameters(params Type[] parameterTypes)
    {
        return Parameters(
            parameterTypes.ConvertAll<Type, ICriteria<ParameterInfo>>(
                static t => new ParameterMatchCriteria()
                {
                    ValueType = Criteria.Equals(t),
                }));
    }

    public TBuilder Parameters(params object?[] arguments)
    {
        _criteria.Parameters = Mirror.GetParameterCriteria(arguments);
        return _builder;
    }

    public TBuilder InheritFrom(MethodBase method)
    {
        base.InheritFrom(method);

        _criteria.GenericTypes = new ArrayEqualityCriteria<Type>(method.GetGenericArguments());
        _criteria.Parameters = Criteria<ParameterInfo>.ArrayOfCriteriaToCriteriaOfArray(method.GetParameters()
            .ConvertAll<ParameterInfo, ICriteria<ParameterInfo>>(static p => new ParameterMatchCriteriaBuilder().InheritFrom(p)));
        return _builder;
    }
}