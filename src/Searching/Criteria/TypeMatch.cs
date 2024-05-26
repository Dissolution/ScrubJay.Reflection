namespace ScrubJay.Reflection.Searching.Criteria;

[Flags]
public enum TypeMatch
{
    Exact = 1 << 0,
    Implements = Exact | 1 << 1,
    ImplementedBy = Exact | 1 << 2,
    
    Any = Exact | Implements | ImplementedBy,
}

public record class TypeMatchCriterion : ICriterion<Type>
{
    public static ICriterion<Type> Create(Type type, TypeMatch match = TypeMatch.Exact)
    {
        return Criterion.Match(type, match);
    }
    public static ICriterion<T> Create<T>(Func<T?, Type?> getType, Type type, TypeMatch match = TypeMatch.Exact)
    {
        return new SubTypeMatchCriterion<T>(Create(type, match), getType);
    }
    
    public Type? Type { get; set; } = null;
    public TypeMatch TypeMatch { get; set; } = TypeMatch.Exact;

    public TypeMatchCriterion() { }
    public TypeMatchCriterion(Type type)
    {
        this.Type = type;
    }
    public TypeMatchCriterion(Type type, TypeMatch typeMatch)
    {
        this.Type = type;
        this.TypeMatch = typeMatch;
    }
    
    public virtual bool Matches(Type? type)
    {
        if (Type is null || type is null)
            return Type is null && type is null && TypeMatch.HasFlags(TypeMatch.Exact);
        
        if (TypeMatch.HasFlags(TypeMatch.Exact))
        {
            if (Type == type) return true;
        }
        if (TypeMatch.HasFlags(TypeMatch.Implements))
        {
            if (type.Implements(Type)) return true;
        }
        if (TypeMatch.HasFlags(TypeMatch.ImplementedBy))
        {
            if (Type.Implements(type)) return true;
        }

        return false;
    }
}

public record class SubTypeMatchCriterion<T> : ICriterion<T>
{
    private readonly ICriterion<Type> _typeMatchCriterion;
    private readonly Func<T?, Type?> _extractType;

    public SubTypeMatchCriterion(ICriterion<Type> typeMatchCriterion, Func<T?, Type?> extractType)
    {
        _typeMatchCriterion = typeMatchCriterion;
        _extractType = extractType;
    }

    public bool Matches(T? value) => _typeMatchCriterion.Matches(_extractType(value));
}