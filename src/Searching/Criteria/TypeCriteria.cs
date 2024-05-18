using ScrubJay.Validation;

namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = 1 << 1,
    ImplementedBy = 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}

public record class TypeCriteria : MemberCriteria, ICriteria<Type>
{
    public static implicit operator TypeCriteria(Type type) => Create(type);
    
    public static TypeCriteria Create(Type type, TypeMatch match = TypeMatch.Exact)
    {
        ThrowIf.Null(type);
        return new()
        {
            Value = type,
            Match = match,
        };
    }
    
    public static new TypeCriteria Create(MemberCriteria criteria)
    {
        return new()
        {
            Access = criteria.Access,
            MemberType = criteria.MemberType,
            Name = criteria.Name,
            Visibility = criteria.Visibility,
        };
    }

    public Type? Value { get; set; } = null;
    public TypeMatch Match { get; set; } = TypeMatch.Exact;
    public GenericTypesCriteria? GenericTypes { get; set; } = null;
    
    public override MemberTypes MemberType => MemberTypes.TypeInfo;

    public bool Matches(Type? type)
    {
        if (!base.Matches(type))
            return false;
        
        if (GenericTypes is not null && !GenericTypes.Matches(type))
            return false;

        if (Value is not null)
        {
            if (type == Value)
                return Match.HasFlags(TypeMatch.Exact);

            if (Match.HasFlags(TypeMatch.Implements) && type.Implements(Value))
                return true;

            if (Match.HasFlags(TypeMatch.ImplementedBy) && Value.Implements(type))
                return true;
            
            return false;
        }

        return true;
    }
}

public abstract class TypeCriteriaBuilder<TBuilder, TCriteria> : MemberCriteriaBuilder<TBuilder, TCriteria>
    where TBuilder : TypeCriteriaBuilder<TBuilder, TCriteria>
    where TCriteria : TypeCriteria, new()
{
    protected TypeCriteriaBuilder() { }
    protected TypeCriteriaBuilder(TCriteria criteria) : base(criteria) { }

    public TBuilder IsType(Type type, TypeMatch match = TypeMatch.Exact)
    {
        _criteria.Value = type;
        _criteria.Match = match;
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

    public TBuilder GenericTypes(params Type[] types) => GenericTypes(types.ConvertAll(static type => TypeCriteria.Create(type)));
}

public sealed class TypeCriteriaBuilder : TypeCriteriaBuilder<TypeCriteriaBuilder, TypeCriteria>, ICriteria<Type>
{
    internal TypeCriteriaBuilder()
    {
    }
    internal TypeCriteriaBuilder(TypeCriteria criteria) : base(criteria)
    {
    }

    public bool Matches(Type? type) => _criteria.Matches(type);
}



public record class GenericTypesCriteria : ICriteria<MethodBase>, ICriteria<Type>
{
    public bool? IsGeneric { get; init; } = null; // null = do not care
    public TypeCriteria[]? TypesCriteria { get; init; } = null;
    
    public bool Matches(Type? type)
    {
        if (type is null) return false;
        if (this.IsGeneric.TryGetValue(out var isGeneric))
        {
            if (type.IsGenericType != isGeneric) return false;
        }
        if (TypesCriteria is not null)
        {
            var genericTypes = type.GetGenericArguments();
            if (TypesCriteria.Length != genericTypes.Length) return false;
            for (var i = 0; i < genericTypes.Length; i++)
            {
                if (!TypesCriteria[i].Matches(genericTypes[i])) return false;
            }
        }
        return true;
    }
    
    public bool Matches(MethodBase? method)
    {
        if (method is null) return false;
        if (this.IsGeneric.TryGetValue(out var isGeneric))
        {
            if (method.IsGenericMethod != isGeneric) return false;
        }
        if (TypesCriteria is not null)
        {
            var genericTypes = method.GetGenericArguments();
            if (TypesCriteria.Length != genericTypes.Length) return false;
            for (var i = 0; i < genericTypes.Length; i++)
            {
                if (!TypesCriteria[i].Matches(genericTypes[i])) return false;
            }
        }
        return true;
    }
}