namespace ScrubJay.Reflection.Searching.Criteria;

public record class MethodBaseCriteria : MemberCriteria, ICriteria<MethodBase>
{
    public static MethodBaseCriteria Create(TypeCriteria returnType)
    {
        return new()
        {
            ReturnType = returnType,
        };
    }
    public static MethodBaseCriteria Create(TypeCriteria returnType, params ParameterCriteria[] parameters)
    {
        return new()
        {
            ReturnType = returnType,
            Parameters = parameters,
        };
    }
    public static new MethodBaseCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }


    public TypeCriteria? ReturnType { get; set; } = null;
    public ParameterCriteria[]? Parameters { get; set; } = null;
    public GenericTypesCriteria? GenericTypes { get; set; } = null;

    public override MemberTypes MemberType => MemberTypes.Method | MemberTypes.Constructor;

    public bool Matches(MethodBase? method)
    {
        if (!base.Matches(method))
            return false;
        if (ReturnType is not null && !ReturnType.Matches(method.ReturnType()))
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
            if (!GenericTypes.Matches(method))
                return false;
        }
        return true;
    }
}

public abstract class MethodBaseCriteriaBuilder<TBuilder, TCriteria> : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : MethodBaseCriteria, new()
{
    protected MethodBaseCriteriaBuilder() { }
    protected MethodBaseCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder ReturnType(Type type)
    {
        _criteria.ReturnType = new(type);
        return _builder;
    }

    public TBuilder ReturnType(TypeCriteria criteria)
    {
        _criteria.ReturnType = criteria;
        return _builder;
    }

    public TBuilder ReturnsVoid()
    {
        _criteria.ReturnType = new(typeof(void));
        return _builder;
    }

    public TBuilder Parameters(params ParameterCriteria[] criteria)
    {
        _criteria.Parameters = criteria;
        return _builder;
    }

    public TBuilder ParameterTypes(params TypeCriteria[] criteria)
    {
        _criteria.Parameters = Array.ConvertAll<TypeCriteria, ParameterCriteria>(criteria, static tc => new(tc));
        return _builder;
    }

    public TBuilder ParameterTypes(params Type[] types)
    {
        _criteria.Parameters = Array.ConvertAll<Type, ParameterCriteria>(types, static type => new(new(type)));
        return _builder;
    }

    public TBuilder ParameterTypes(params object[] arguments)
    {
        _criteria.Parameters = Array.ConvertAll<object, ParameterCriteria>(arguments, static obj => new(new(obj.GetType())));
        return _builder;
    }

    public TBuilder NoParameters()
    {
        _criteria.Parameters = Array.Empty<ParameterCriteria>();
        return _builder;
    }


    public TBuilder Generic
    {
        get
        {
            _criteria.GenericTypes = new GenericTypesCriteria() { IsGeneric = true };
            return _builder;
        }
    }

    public TBuilder NonGeneric
    {
        get
        {
            _criteria.GenericTypes = new GenericTypesCriteria() { IsGeneric = false };
            return _builder;
        }
    }

    public TBuilder GenericTypes(params TypeCriteria[] criteria)
    {
        _criteria.GenericTypes = new GenericTypesCriteria()
        {
            IsGeneric = true,
            TypesCriteria = criteria,
        };
        return _builder;
    }

    public TBuilder GenericTypes(params Type[] types) => GenericTypes(types.ConvertAll(static type => new TypeCriteria(type)));
}

public sealed class MethodBaseCriteriaBuilder : MethodBaseCriteriaBuilder<MethodBaseCriteriaBuilder, MethodBaseCriteria>, ICriteria<MethodBase>
{
    internal MethodBaseCriteriaBuilder()
    {
    }
    internal MethodBaseCriteriaBuilder(MethodBaseCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(MethodBase? method) => _criteria.Matches(method);
}